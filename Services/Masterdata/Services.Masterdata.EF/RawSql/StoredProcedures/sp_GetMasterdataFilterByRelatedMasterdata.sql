--efcore.migration.down
--DROP PROCEDURE IF EXISTS [dbo].sp_GetMasterdataFilterByRelatedMasterdata

--efcore.migration.up
CREATE OR ALTER PROC [dbo].sp_GetMasterdataFilterByRelatedMasterdata
    @masterdataType AS nvarchar(50),
    @filterMasterdata AS nvarchar(max),
    @includeDescendants AS bit = 0
AS BEGIN

-- DECLARE @masterdataType AS nvarchar(50) = 'status';
-- DECLARE @filterMasterdata AS nvarchar(max) = 'uom:kg';
-- DECLARE @filterMasterdata AS nvarchar(max) = '2f2d5cd5-f3b0-ed11-baff-001a7dda7110:e36f077b-f4b0-ed11-baff-001a7dda7110';
-- DECLARE @includeDescendants AS bit = 1;

-- CREATE FILTER TABLE CTE
WITH cteFilter(masterdataType, masterdataTypeIsId, masterdata, masterdataIsId) AS 
(
SELECT
    PARSENAME(REPLACE([value], ':', '.'), 2) AS masterdataType
  , CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, PARSENAME(REPLACE([value], ':', '.'), 2)) IS NULL THEN 0 ELSE 1 END AS masterdataTypeIsId
  , PARSENAME(REPLACE([value], ':', '.'), 1) AS masterdata
  , CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, PARSENAME(REPLACE([value], ':', '.'), 1)) IS NULL THEN 0 ELSE 1 END AS masterdataIsId
FROM
    STRING_SPLIT (@filterMasterdata, ',')
)

-- CREATE SELECT MASTERDATA RECURSIVE CTE
, cte(Id, [Key], Name, MasterdataTypeId, [level], [path]) AS
(
SELECT 
      mdChild.Id
    , mdChild.[Key]
    , mdChild.Name
    , mdChild.MasterdataTypeId
    , 1
    , CAST(mdChild.[Key] AS NVARCHAR(MAX))
FROM
    Masterdatas mdChild
    INNER JOIN MasterdataTypes mt ON mt.Id = mdChild.MasterdataTypeId
WHERE 
    1 = 1
    AND mt.Code = CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, @masterdataType) IS NULL THEN @masterdataType ELSE mt.Code END
    AND mdChild.MasterdataTypeId = ISNULL(TRY_CONVERT(UNIQUEIDENTIFIER, @masterdataType), mdChild.MasterdataTypeId)
    AND EXISTS (
        SELECT 1
        FROM MasterdataRelated AS m1
        INNER JOIN Masterdatas AS md1 ON md1.Id = m1.ChildMasterdataId
        INNER JOIN MasterdataTypes mt1 ON mt1.Id = md1.MasterdataTypeId
        , cteFilter AS fltr
        WHERE 
            1 = 1
            AND m1.ParentMasterdataId = mdChild.Id
            AND mt1.Code = CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, fltr.MasterdataType) IS NULL THEN fltr.MasterdataType ELSE mt1.Code END
            AND md1.MasterdataTypeId = ISNULL(TRY_CONVERT(UNIQUEIDENTIFIER, fltr.MasterdataType), md1.MasterdataTypeId)
            AND md1.[Key] = CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, fltr.Masterdata) IS NULL THEN fltr.Masterdata ELSE md1.[Key] END
            AND m1.ChildMasterdataId = ISNULL(TRY_CONVERT(UNIQUEIDENTIFIER, fltr.Masterdata), m1.ChildMasterdataId)

        )
UNION ALL
SELECT
      mdParent.Id
    , mdParent.[Key]
    , mdParent.Name
    , mdParent.MasterdataTypeId
    , childCte.[level] + 1
    , childCte.[path] + ' -> ' + mdParent.[Key]
FROM
    Masterdatas mdParent
    INNER JOIN MasterdataRelated r ON r.ParentMasterdataId = mdParent.Id
    INNER JOIN cte childCte ON childCte.Id = r.ChildMasterdataId AND childCte.MasterdataTypeId = mdParent.MasterdataTypeId
WHERE
    1 = @includeDescendants
)

-- RETURN DE-DUPLICATED RESULT
SELECT 
      Id
    , [Key]
    , Name
    , MAX(level)    AS [level]
    , MAX(path)     AS [path]
FROM cte
GROUP BY Id, [Key], Name

END