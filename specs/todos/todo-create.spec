# Todo Create
* テーブル "todos" のデータを全て削除する

## Todoが作成できる
* URL "/Todos/Create" を開く
* 要素 "input[name='Title']" に "New Todo" と入力する
* 要素 "button[type='submit']" をクリックする
* URL "/Todos" に遷移している
* 要素 "tbody tr" が "1" 件表示されている
* テーブル "todos" の内容が <table:expected/todo/db/todo-create.csv> と一致している