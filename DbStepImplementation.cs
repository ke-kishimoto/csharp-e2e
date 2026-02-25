#nullable enable
using Gauge.CSharp.Lib;
using Gauge.CSharp.Lib.Attribute;
using System.Security.Permissions;

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
    }
}