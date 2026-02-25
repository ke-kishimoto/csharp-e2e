#nullable enable
using Gauge.CSharp.Lib;
using Gauge.CSharp.Lib.Attribute;
using Shouldly;
using System.Collections.Generic;

namespace DotNet.Template
{
    /// <summary>
    /// Gauge のステップ実装クラス。
    /// データベース操作のステップを定義しています。
    /// </summary>
    public class DbStepImplementation
    {
        [Step("テーブル <table> のデータを全て削除する")]
        public void DeleteAllDataFromTable(string table)
        {
            DatabaseHelper.TruncateTableAsync(table).GetAwaiter().GetResult();
        }

        [Step("CSVファイル <csvPath> の内容をテーブル <table> に投入する")]
        public void InsertDataFromCsvToTable(string csvPath, string table)
        {
            DatabaseHelper.InsertFromCsvAsync(tableName: table, relativePath: csvPath).GetAwaiter().GetResult();
        }

        [Step("テーブル <table> の内容が <csv> と一致している")]
        public void TableContentIs(string table, Table csv)
        {
            var expectedRows = csv.GetTableRows();
            if (expectedRows.Count == 0) return;

            // Gauge Table の列名一覧を取得
            // 1列目を ORDER BY キーとして使用し、結果を決定的な順序にする
            var columns    = csv.GetColumnNames();
            var orderByCol = columns.Count > 0 ? columns[0] : null;

            var actualRows = DatabaseHelper
                .QueryTableRowsAsync(table, orderByCol)
                .GetAwaiter().GetResult();

            // 行数チェック
            actualRows.Count.ShouldBe(expectedRows.Count,
                $"テーブル '{table}' の行数が一致しません。期待値: {expectedRows.Count} 件、実際: {actualRows.Count} 件");

            // 行ごとに、Gauge Table に存在する列だけ比較する
            for (var i = 0; i < expectedRows.Count; i++)
            {
                var expectedRow = expectedRows[i];
                var actualRow   = actualRows[i];

                foreach (var col in columns)
                {
                    var expectedValue = expectedRow.GetCell(col)?.Trim() ?? string.Empty;
                    actualRow.TryGetValue(col, out var rawActual);
                    var actualValue = rawActual?.Trim() ?? string.Empty;

                    actualValue.ShouldBe(expectedValue,
                        $"行 {i + 1}、列 '{col}' の値が一致しません");
                }
            }
        }
    }
}