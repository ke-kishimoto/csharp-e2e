#nullable enable
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNet.Template
{
    /// <summary>
    /// DB テストデータ管理ユーティリティ。
    /// SQL ファイルの実行と CSV ファイルからのデータ投入をサポートします。
    /// ファイルパスはプロジェクトルートからの相対パス、または絶対パスで指定できます。
    /// </summary>
    public static class DatabaseHelper
    {
        // GO はバッチ区切り (大文字小文字・前後空白を無視)
        private static readonly Regex GoBatchSeparator =
            new(@"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// SQL ファイルを実行します。
        /// T-SQL の GO バッチ区切りに対応しています。
        /// </summary>
        /// <param name="relativePath">プロジェクトルートからの相対パス (例: testdata/sql/seed.sql)</param>
        public static async Task ExecuteSqlFileAsync(string relativePath)
        {
            var fullPath = ResolvePath(relativePath);
            var sql = await File.ReadAllTextAsync(fullPath);
            await ExecuteSqlAsync(sql);
        }

        /// <summary>
        /// SQL 文字列を直接実行します。
        /// T-SQL の GO バッチ区切りに対応しています。
        /// </summary>
        /// <param name="sql">実行する SQL 文字列</param>
        public static async Task ExecuteSqlAsync(string sql)
        {
            var config = DbConfig.Load();

            await using var conn = new SqlConnection(config.ConnectionString);
            await conn.OpenAsync();

            // GO で分割して各バッチを順番に実行する
            var batches = GoBatchSeparator.Split(sql);
            foreach (var batch in batches)
            {
                var trimmed = batch.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                await using var cmd = new SqlCommand(trimmed, conn)
                {
                    CommandTimeout = config.CommandTimeout
                };
                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// CSV ファイルからデータを読み込み、指定のテーブルに一括挿入します。
        /// CSV の1行目をヘッダー (列名) として扱います。
        /// </summary>
        /// <param name="tableName">挿入先のテーブル名</param>
        /// <param name="relativePath">プロジェクトルートからの相対パス (例: testdata/csv/todos.csv)</param>
        public static async Task InsertFromCsvAsync(string tableName, string relativePath)
        {
            var fullPath = ResolvePath(relativePath);
            var dataTable = ReadCsvToDataTable(fullPath);
            await BulkInsertAsync(tableName, dataTable);
        }

        /// <summary>
        /// 指定テーブルの全データを削除します。
        /// テストデータの初期化に使用します。
        /// </summary>
        /// <param name="tableName">削除対象のテーブル名</param>
        public static async Task TruncateTableAsync(string tableName)
        {
            // テーブル名を角括弧で囲み SQL インジェクションを防ぐ
            var safeName = EscapeTableName(tableName);
            await ExecuteSqlAsync($"TRUNCATE TABLE {safeName};");
        }

        // -------------------------------------------------------------------

        private static DataTable ReadCsvToDataTable(string fullPath)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,   // 列数が不足している行はスキップ
            };

            using var reader = new StreamReader(fullPath);
            using var csv = new CsvReader(reader, csvConfig);

            var dataTable = new DataTable();
            bool headersAdded = false;

            while (csv.Read())
            {
                if (!headersAdded)
                {
                    csv.ReadHeader();
                    foreach (var header in csv.HeaderRecord ?? Array.Empty<string>())
                        dataTable.Columns.Add(header);
                    headersAdded = true;
                    continue;
                }

                var row = dataTable.NewRow();
                foreach (DataColumn col in dataTable.Columns)
                {
                    var raw = csv.GetField(col.ColumnName);
                    row[col.ColumnName] = string.IsNullOrEmpty(raw) ? DBNull.Value : (object)raw;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static async Task BulkInsertAsync(string tableName, DataTable dataTable)
        {
            var config = DbConfig.Load();
            var safeName = EscapeTableName(tableName);

            await using var conn = new SqlConnection(config.ConnectionString);
            await conn.OpenAsync();

            using var bulk = new SqlBulkCopy(conn)
            {
                DestinationTableName = safeName,
                BulkCopyTimeout      = config.CommandTimeout,
            };

            // CSV のヘッダー名とテーブルの列名をマッピング
            foreach (DataColumn col in dataTable.Columns)
                bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

            await bulk.WriteToServerAsync(dataTable);
        }

        /// <summary>テーブル名をスキーマ付きで角括弧エスケープする</summary>
        private static string EscapeTableName(string tableName)
        {
            // "dbo.TableName" や "TableName" の形式に対応
            var parts = tableName.Split('.');
            var escaped = new List<string>();
            foreach (var part in parts)
                escaped.Add($"[{part.Trim('[', ']')}]");
            return string.Join(".", escaped);
        }

        /// <summary>相対パスをプロジェクトルート基準の絶対パスに解決する</summary>
        private static string ResolvePath(string path) =>
            Path.IsPathRooted(path)
                ? path
                : Path.Combine(Directory.GetCurrentDirectory(), path);
    }
}
