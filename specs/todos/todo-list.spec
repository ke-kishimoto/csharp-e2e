# Todo list
* テーブル "todos" のデータを全て削除する
* CSVファイル "testdata/csv/todos.csv" の内容をテーブル "todos" に投入する

## 一覧が表示されている CSVver
* URL "/todos" を開く
* 要素 "h1" に "Todos" が表示されている
* 要素 "tbody tr" が "3" 件表示されている
* テーブル要素 "table" の内容が <table:expected/todo/todo-list.csv> と一致している

## 一覧が表示されている TableVer

|row|ID|Title       |Done |Created           |
|---|--|------------|-----|------------------|
|1  |3 |Third Todo  |False|2024/06/04 8:00:00|
|2  |2 |Another Todo|True |2024/06/03 6:00:00|
|3  |1 |Sample Todo |False|2024/06/02 4:00:00|

* URL "/todos" を開く
* 要素 "h1" に "Todos" が表示されている
* 要素 "tbody tr" が "3" 件表示されている
* テーブル要素 "table" の <row> 行目の "ID" 列の値が <ID> である
* テーブル要素 "table" の <row> 行目の "Title" 列の値が <Title> である
* テーブル要素 "table" の <row> 行目の "Done" 列の値が <Done> である
* テーブル要素 "table" の <row> 行目の "Created" 列の値が <Created> である

## 新規作成画面に遷移する
* URL "/todos" を開く
* リンク "Create New" をクリックする
* URL "/Todos/Create" に遷移している
* 要素 "h1" に "Create Todo" が表示されている

