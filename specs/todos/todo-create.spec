# Todo Create
* テーブル "todos" のデータを全て削除する

## Todoが作成できる CSV確認ver
* URL "/Todos/Create" を開く
* 要素 "input[name='Title']" に "New Todo" と入力する
* 要素 "button[type='submit']" をクリックする
* URL "/Todos" に遷移している
* 要素 "tbody tr" が "1" 件表示されている
* テーブル "todos" の内容が <table:expected/todo/db/todo-create.csv> と一致している

## Todoが作成できる Table確認ver1
* URL "/Todos/Create" を開く
* 要素 "input[name='Title']" に "New Todo" と入力する
* 要素 "button[type='submit']" をクリックする
* URL "/Todos" に遷移している
* 要素 "tbody tr" が "1" 件表示されている
* テーブル "todos" の内容が以下の通りである
|Id|Title|Done|
|--|-----|----|
|1 |New Todo|False|

## Todoが作成できる Table確認ver2
* URL "/Todos/Create" を開く
* 要素 "input[name='Title']" に "New Todo" と入力する
* 要素 "button[type='submit']" をクリックする
* URL "/Todos" に遷移している
* テーブル "todos" の条件 "Id = 1" のレコードの内容が以下の通りである
|Column|Value   |
|------|--------|
|Id    |1       |
|Title |New Todo|
|Done  |False   |
