-- Histórico das análises da calculadora CAPAG (seletor de modelos)

CREATE TABLE IF NOT EXISTS `capag_calculadora_resultado` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `id_tenant` BIGINT UNSIGNED NOT NULL COMMENT 'Chave de isolamento.',
  `id_empresa` BIGINT UNSIGNED NOT NULL,
  `modelo` ENUM('capag-e-1', 'capag-e-2', 'capag-p') NOT NULL COMMENT 'Modelo da calculadora selecionada',
  `classificacao` CHAR(1) NOT NULL COMMENT 'Rating exibido (A, B, C ou D)',
  `percentual_exibicao` VARCHAR(16) NOT NULL COMMENT 'Percentual formatado exibido no card (ex.: 100,00%)',
  `label_metrica` VARCHAR(64) NOT NULL COMMENT 'Rótulo da métrica (ex.: ICP Final)',
  `status_mensagem` VARCHAR(255) NOT NULL COMMENT 'Texto do chip de status (ex.: CAPAG em conformidade...)',
  `valor_capag` DECIMAL(20, 4) NOT NULL COMMENT 'Valor CAPAG usado no cálculo do índice',
  `valor_divida` DECIMAL(20, 4) NULL COMMENT 'Valor da dívida usado no cálculo do índice',
  `indice` DECIMAL(18, 6) NULL COMMENT 'Índice CAPAG/dívida (ex.: 16.96)',
  `parcial` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '1 = cálculo em andamento; 0 = análise concluída',
  `date_create` DATETIME NOT NULL,
  `date_update` DATETIME NOT NULL,
  `id_usuario` BIGINT NULL COMMENT 'Usuário Capag que criou/atualizou',

  PRIMARY KEY (`id`),
  KEY `idx_capag_calculadora_resultado_tenant` (`id_tenant`),
  KEY `idx_capag_calculadora_resultado_empresa` (`id_empresa`),
  KEY `idx_capag_calculadora_resultado_empresa_modelo_data` (`id_empresa`, `modelo`, `date_create`),
  KEY `idx_capag_calculadora_resultado_usuario` (`id_usuario`),

  CONSTRAINT `fk_capag_calculadora_resultado_tenant`
    FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`)
    ON DELETE RESTRICT ON UPDATE RESTRICT,

  CONSTRAINT `fk_capag_calculadora_resultado_empresa`
    FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
    ON DELETE RESTRICT ON UPDATE CASCADE,

  CONSTRAINT `chk_capag_calculadora_resultado_classificacao`
    CHECK (`classificacao` IN ('A', 'B', 'C', 'D'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Histórico das análises da calculadora CAPAG (um registro por cálculo)';
