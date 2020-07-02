CREATE USER 'admin'@'%' IDENTIFIED BY 'Acount2019#$';
GRANT
    SELECT,
    DELETE,
    EXECUTE,
    INSERT,
    UPDATE
ON `books`.* TO 'admin'@'%';
