# Todo API

## Todoリストを取得できる
* テーブル "todos" のデータを全て削除する
* SQL <file:testdata/sql/todos.sql> を実行する
* "/api/todos" にGETリクエストを送る
* ステータスコードが "200" である
* レスポンスのJSONが <file:expected/todo/json/todos.json> と一致している
