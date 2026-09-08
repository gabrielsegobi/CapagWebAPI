-- Inclui o status comercial 'em_andamento' no comentário da coluna

ALTER TABLE `empresas`
  MODIFY COLUMN `status` VARCHAR(30) NULL
    COMMENT 'Status comercial: calculo_efetuado | em_andamento | em_negociacao | contrato_fechado';
