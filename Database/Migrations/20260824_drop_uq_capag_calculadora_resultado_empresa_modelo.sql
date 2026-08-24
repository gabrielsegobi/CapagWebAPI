-- Histórico de análises: permite múltiplos resultados por empresa e modelo

ALTER TABLE `capag_calculadora_resultado`
  DROP INDEX `uq_capag_calculadora_resultado_empresa_modelo`,
  ADD INDEX `idx_capag_calculadora_resultado_empresa_modelo_data` (`id_empresa`, `modelo`, `date_create`);

ALTER TABLE `capag_calculadora_resultado`
  COMMENT = 'Histórico das análises da calculadora CAPAG (um registro por cálculo)';
