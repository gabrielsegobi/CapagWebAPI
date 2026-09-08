-- Renomeia capag_calculadora_resultado → calculadora_resultado
-- (ambientes que já aplicaram 20260807 / 20260824)

RENAME TABLE `capag_calculadora_resultado` TO `calculadora_resultado`;

ALTER TABLE `calculadora_resultado`
  RENAME INDEX `idx_capag_calculadora_resultado_tenant` TO `idx_calculadora_resultado_tenant`,
  RENAME INDEX `idx_capag_calculadora_resultado_empresa` TO `idx_calculadora_resultado_empresa`,
  RENAME INDEX `idx_capag_calculadora_resultado_empresa_modelo_data` TO `idx_calculadora_resultado_empresa_modelo_data`,
  RENAME INDEX `idx_capag_calculadora_resultado_usuario` TO `idx_calculadora_resultado_usuario`;

ALTER TABLE `calculadora_resultado`
  DROP FOREIGN KEY `fk_capag_calculadora_resultado_tenant`,
  DROP FOREIGN KEY `fk_capag_calculadora_resultado_empresa`;

ALTER TABLE `calculadora_resultado`
  ADD CONSTRAINT `fk_calculadora_resultado_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,
  ADD CONSTRAINT `fk_calculadora_resultado_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE;

ALTER TABLE `calculadora_resultado`
  DROP CHECK `chk_capag_calculadora_resultado_classificacao`,
  ADD CONSTRAINT `chk_calculadora_resultado_classificacao`
    CHECK (`classificacao` IN ('A', 'B', 'C', 'D'));
