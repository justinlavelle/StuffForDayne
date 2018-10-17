-- =============================================
-- Author:		Justin Lavelle
-- Create date: 2016/09/30
-- Description:	Insert data into the Translation table
-- =============================================
CREATE PROCEDURE [dbo].[SP_InsertTranslation]
	@MASTERSOURCEPHRASEID BIGINT,
	@TEXT VARCHAR(MAX),
	@COUNTRYISO VARCHAR(2),
	@LANGUAGEISO VARCHAR(2),
	@ID BIGINT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    --declare locale for en
	DECLARE @LOCALEID INT = (SELECT [dbo].[FN_GetLocaleIdByIsoCode] (@COUNTRYISO, @LANGUAGEISO))

	--transform text
	SET @TEXT = (SELECT [dbo].[FN_ReplaceWithReservedPhrases] (@TEXT))

	--check whether text exists
	SET @ID = (SELECT TOP 1 [Id]
			  FROM [dbo].[Translation]
			  WHERE [MasterSourcePhraseId] = @MASTERSOURCEPHRASEID
			    AND [Text] COLLATE Latin1_General_CS_AS = @TEXT
				AND [LocaleId] = @LOCALEID)

	--prevent duplicates
	IF (@ID IS NULL)
	BEGIN

		--add translation text
		INSERT INTO [dbo].[Translation]
			   ([MasterSourcePhraseId]
			   ,[LocaleId]
			   ,[Text])
		 VALUES
			   (@MASTERSOURCEPHRASEID
			   ,@LOCALEID
			   ,@TEXT)

		--get the newly added id
		SET @ID = SCOPE_IDENTITY()

		--print out data
		PRINT N'Inserted translation value "' + @TEXT + '" for ' + CAST(@LOCALEID AS NVARCHAR(30))

	END

END