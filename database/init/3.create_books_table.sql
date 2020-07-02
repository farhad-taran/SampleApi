CREATE TABLE `books`.`book_items` (
  `book_id` varchar(45) NOT NULL,
  `author_id` varchar(200) NOT NULL,
  `name` varchar(200) NOT NULL,
  PRIMARY KEY (`book_id`));