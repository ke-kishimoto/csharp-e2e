# Todo Stored Procedure

## ストアドでTodoリストを更新できる
* テーブル "todos" のデータを全て削除する
* テーブル "todos" に <table:testdata/csv/todos.csv> の内容を投入する
* ストアドプロシージャ "ups_Todos_BulkUpdate" を実行する
* テーブル "todos" の内容が <table:expected/todo/db/todo-updated.csv> と一致している

