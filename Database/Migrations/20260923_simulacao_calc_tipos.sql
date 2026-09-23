-- =============================================================================
-- simulacao_calc.tipo_simulacao — VARCHAR no lugar de ENUM
-- Produção | MySQL 8+ | schema: gsaas
-- ENUM rejeitava naturezas novas (ex.: FGTS) com Data truncated → 500.
-- =============================================================================

USE `gsaas`;

ALTER TABLE `simulacao_calc`
  MODIFY COLUMN `tipo_simulacao` VARCHAR(32) NOT NULL;
