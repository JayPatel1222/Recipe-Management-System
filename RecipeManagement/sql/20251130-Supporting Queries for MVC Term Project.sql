/*
* NAME:		Team10 - Jay Patel & Vianey Gandara
* DATE:		Oct/2025
* PURPOSE:	Supporing queries for MVC TermProject
* CLASS:	Software Developer Sr. B
*/

USE [mvc2025TermProject];
GO

SELECT * FROM Recipes;

SELECT 
	RecipeId, 
	[Name],
	Quantity,
	Unit
FROM [dbo].[RecipeIngredients] 
INNER JOIN Ingredients ON IngredientId = Ingredients.Id ;
/*
10 L - Milk

3 kg - Carrots

3 kg - Onion

50 g - Pork*/ 

SELECT * FROM [dbo].[IngredientTypes];



SELECT * FROM  Ingredients; 

SELECT * FROM  Categories; 

SELECT * FROM AspNetUsers --WHERE Id = '8d41bed4-20a5-402f-89bd-9187c6748b0d'; 

SELECT * FROM AspNetRoles;

SELECT NEWID();

INSERT INTO AspNetRoles(ID, Name, NormalizedName)
VALUES
	('BBC11177-F580-4F47-857D-F4C67D6AF965', 'NormalUser', 'NORMALUSER');

INSERT INTO AspNetRoles(ID, Name, NormalizedName)
VALUES
	('40437663-03D1-48DD-A9EB-B2C88B7E02E1', 'Admin', 'ADMIN');

--DELETE FROM AspNetRoles WHERE ID = '40437663-03D1-48DD-A9EB-B2C88B7E02E1';



INSERT INTO AspNetUserRoles(RoleId,  UserId)
VALUES	
	('BBC11177-F580-4F47-857D-F4C67D6AF965', '8d41bed4-20a5-402f-89bd-9187c6748b0d');

--DELETE FROM AspNetUserRoles WHERE RoleId = '40437663-03D1-48DD-A9EB-B2C88B7E02E1';
SELECT * FROM AspNetUserRoles;

INSERT INTO AspNetUserRoles(RoleId,  UserId)
VALUES	
	('40437663-03D1-48DD-A9EB-B2C88B7E02E1', '9328eeea-882e-4f14-b5f0-7ee708850246');


-- DROP DATABASE [mvc2025TermProject];
--GO

INSERT INTO IngredientTypes (Name) 
VALUES 
	('Dairy'),
	('Produce'), 
	('Grain');

INSERT INTO IngredientTypes (Name) 
VALUES 
	('Meat'),
	('Fat');

SELECT * FROM [dbo].[IngredientTypes];


UPDATE AspNetUsers SET UserName = 'user3@gmail.com' WHERE Id = '30f98d0e-0fee-40f9-91cc-ff23c4d8df82';
UPDATE AspNetUsers SET Email = 'user333@gmail.com' WHERE Id = '30f98d0e-0fee-40f9-91cc-ff23c4d8df82';


SELECT 
	AspNetUsers.FirstName, 
	AspNetRoles.Name
FROM AspNetUserRoles 
	INNER JOIN AspNetUsers ON AspNetUsers.Id = UserId 
	INNER JOIN AspNetRoles ON RoleId = AspNetRoles.Id;

SELECT 
	Recipes.Name, 
	AspNetUsers.FirstName,
	UserId
FROM Recipes INNER JOIN AspNetUSers 
ON AspNetUSers.Id = Recipes.UserId

SELECT * FROM AspNetUsers WHERE Id = '4e2d7e43-ba4b-44ce-a704-7e09b83e6f42';

UPDATE AspNetUsers 
SET 
	FirstName = 'Vianey', 
	LastName = 'Ggg', 
	Municipality = 'NB', 
	StreetAddress = '123 St', 
	PhoneNumber = '555-123-1212'
WHERE Id = '4e2d7e43-ba4b-44ce-a704-7e09b83e6f42';

SELECT 
	Recipes.Name,
	Recipes.CategoryId, 
	Recipes.CreationDate,
	Ingredients.Name
FROM Recipes 
	INNER JOIN RecipeIngredients ON RecipeId = Recipes.Id 
	INNER JOIN Ingredients ON IngredientId = Ingredients.Id;

SELECT * FROM RecipeIngredients; 

SELECT * FROM Images; 

DELETE FROM Images WHERE Id IN (8, 9);


UPDATE Images
SET 
    Status = 1
WHERE 
    RecipeID = 5;

