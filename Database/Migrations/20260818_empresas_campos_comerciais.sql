-- Novos campos comerciais na tabela empresas

ALTER TABLE `empresas`
  ADD COLUMN `status` VARCHAR(30) NULL
    COMMENT 'Status comercial: calculo_efetuado | em_negociacao | contrato_fechado'
    AFTER `data_impedimento`,
  ADD COLUMN `valor_contrato` DECIMAL(20,2) NULL
    COMMENT 'Valor estimado / fechado do contrato de captacao'
    AFTER `status`;
