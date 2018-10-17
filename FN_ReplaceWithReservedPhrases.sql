-- =============================================
-- Author:		Justin Lavelle
-- Create date: 2016/09/30
-- Description:	To replace all the reserved phrases.
-- =============================================
CREATE FUNCTION [dbo].[FN_ReplaceWithReservedPhrases]
(
	@TEXT VARCHAR(MAX)
)
RETURNS VARCHAR(MAX)
AS
BEGIN

	DECLARE @CURRENTID INT = (SELECT MIN([Id]) FROM [Translations].[dbo].[ReservedPhrases])

	WHILE(@CURRENTID > 0)
	BEGIN
		DECLARE @RESERVEDPHRASE VARCHAR(MAX) = (SELECT [ReservedPhrase]		
												FROM [Translations].[dbo].[ReservedPhrases] 
												WHERE [Id] = @CURRENTID)

		DECLARE @REPLACEMENT VARCHAR(MAX) = '!*' + CONVERT(VARCHAR(MAX), @CURRENTID) + '*!'

		--replace 100% matching configurations
		SET @TEXT = CASE WHEN @RESERVEDPHRASE = @TEXT
						  THEN @REPLACEMENT
						 WHEN @RESERVEDPHRASE + '.' = @TEXT
						  THEN @REPLACEMENT + '.'
						 WHEN @RESERVEDPHRASE + ',' = @TEXT
						  THEN @REPLACEMENT + ','
						 WHEN @RESERVEDPHRASE + '?' = @TEXT
						  THEN @REPLACEMENT + '?'
						 WHEN @RESERVEDPHRASE + '!' = @TEXT
						  THEN @REPLACEMENT + '!'
						 WHEN @RESERVEDPHRASE + '™' = @TEXT
						  THEN @REPLACEMENT + '™'
					  ELSE @TEXT 
					 END

		--replace known configurations
		SET @TEXT = REPLACE(@TEXT,' ' + @RESERVEDPHRASE + ' ',' ' + @REPLACEMENT + ' ')
		SET @TEXT = REPLACE(@TEXT,' ' + @RESERVEDPHRASE + '.',' ' + @REPLACEMENT + '.')
		SET @TEXT = REPLACE(@TEXT,' ' + @RESERVEDPHRASE + ',',' ' + @REPLACEMENT + ',')
		SET @TEXT = REPLACE(@TEXT,' ' + @RESERVEDPHRASE + '?',' ' + @REPLACEMENT + '?')
		SET @TEXT = REPLACE(@TEXT,' ' + @RESERVEDPHRASE + '!',' ' + @REPLACEMENT + '!')
		SET @TEXT = REPLACE(@TEXT,' ' + @RESERVEDPHRASE + '™',' ' + @REPLACEMENT + '™')

		--replace starts with configurations
		SET @TEXT = CASE WHEN CHARINDEX(@RESERVEDPHRASE + ' ', @TEXT) = 1
						  THEN STUFF(@TEXT, 1, Len(@RESERVEDPHRASE), @REPLACEMENT)
						 WHEN CHARINDEX(@RESERVEDPHRASE + '. ', @TEXT) = 1 
						  THEN STUFF(@TEXT, 1, Len(@RESERVEDPHRASE), @REPLACEMENT)
						 WHEN CHARINDEX(@RESERVEDPHRASE + ', ', @TEXT) = 1 
						  THEN STUFF(@TEXT, 1, Len(@RESERVEDPHRASE), @REPLACEMENT)
						 WHEN CHARINDEX(@RESERVEDPHRASE + '? ', @TEXT) = 1 
						  THEN STUFF(@TEXT, 1, Len(@RESERVEDPHRASE), @REPLACEMENT)
						 WHEN CHARINDEX(@RESERVEDPHRASE + '! ', @TEXT) = 1 
						  THEN STUFF(@TEXT, 1, Len(@RESERVEDPHRASE), @REPLACEMENT)
						 WHEN CHARINDEX(@RESERVEDPHRASE + '™ ', @TEXT) = 1 
						  THEN STUFF(@TEXT, 1, Len(@RESERVEDPHRASE), @REPLACEMENT)
					  ELSE @TEXT 
					 END

		--replace ends with configurations
		SELECT @TEXT = CASE 
			WHEN CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE), REVERSE(@TEXT)) = 1 
				THEN REVERSE(STUFF(REVERSE(@TEXT), CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE), REVERSE(@TEXT)), LEN(' ' + @RESERVEDPHRASE), REVERSE(' ' + @REPLACEMENT)))
			WHEN CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '.'), REVERSE(@TEXT)) = 1 
				THEN REVERSE(STUFF(REVERSE(@TEXT), CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '.'), REVERSE(@TEXT)), LEN(' ' + @RESERVEDPHRASE+ '.'), REVERSE(' ' + @REPLACEMENT + '.')))
			WHEN CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '?'), REVERSE(@TEXT)) = 1 
				THEN REVERSE(STUFF(REVERSE(@TEXT), CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '?'), REVERSE(@TEXT)), LEN(' ' + @RESERVEDPHRASE+ '?'), REVERSE(' ' + @REPLACEMENT + '?')))
			WHEN CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '!'), REVERSE(@TEXT)) = 1 
				THEN REVERSE(STUFF(REVERSE(@TEXT), CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '!'), REVERSE(@TEXT)), LEN(' ' + @RESERVEDPHRASE+ '!'), REVERSE(' ' + @REPLACEMENT + '!')))
			WHEN CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '™'), REVERSE(@TEXT)) = 1 
				THEN REVERSE(STUFF(REVERSE(@TEXT), CHARINDEX(REVERSE(' ' + @RESERVEDPHRASE + '™'), REVERSE(@TEXT)), LEN(' ' + @RESERVEDPHRASE+ '™'), REVERSE(' ' + @REPLACEMENT + '™')))
			ELSE @TEXT
			 END
	
				SET @CURRENTID = (SELECT MIN([Id]) FROM [Translations].[dbo].[ReservedPhrases] WHERE [Id] > @CURRENTID)
			END

	-- Return the result of the function
	RETURN @TEXT

END