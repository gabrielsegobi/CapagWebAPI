-- Execute com a API parada para impedir novas gravações durante a migração.
-- Mantém ativo o indicador mais recente de cada tenant/empresa/nome.

ALTER TABLE `indicadores`
    ADD INDEX `idx_indicadores_empresa` (`id_empresa`);

ALTER TABLE `indicadores`
    DROP INDEX `uk_indicadores_empresa_nome`;

UPDATE `indicadores` AS `indicador`
INNER JOIN (
    SELECT
        `id_tenant`,
        `id_empresa`,
        `nome`,
        MAX(`id_indicador`) AS `id_indicador_manter`
    FROM `indicadores`
    WHERE `deleted_at` IS NULL
    GROUP BY `id_tenant`, `id_empresa`, `nome`
    HAVING COUNT(*) > 1
) AS `duplicados`
    ON `duplicados`.`id_tenant` = `indicador`.`id_tenant`
    AND `duplicados`.`id_empresa` = `indicador`.`id_empresa`
    AND `duplicados`.`nome` = `indicador`.`nome`
SET
    `indicador`.`deleted_at` = CURRENT_TIMESTAMP,
    `indicador`.`updated_at` = CURRENT_TIMESTAMP
WHERE `indicador`.`deleted_at` IS NULL
  AND `indicador`.`id_indicador` <> `duplicados`.`id_indicador_manter`;

ALTER TABLE `indicadores`
    ADD COLUMN `ativo_unico` TINYINT
        GENERATED ALWAYS AS (
            CASE WHEN `deleted_at` IS NULL THEN 1 ELSE NULL END
        ) STORED,
    ADD UNIQUE INDEX `uk_indicadores_ativo`
        (`id_tenant`, `id_empresa`, `nome`, `ativo_unico`);
