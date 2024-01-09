SELECT * FROM mydatabase.users;
create database MyDatabase;
USE MyDatabase;

CREATE TABLE IF NOT EXISTS Users (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255),
    Email VARCHAR(255),
    Password VARCHAR(255),
    Processed  TINYINT(1) DEFAULT 0
  
);