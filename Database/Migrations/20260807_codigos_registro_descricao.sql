-- Códigos de registro descrição (tenant + empresa)

CREATE TABLE IF NOT EXISTS `codigos_registro_descricao` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `id_tenant` bigint unsigned NOT NULL COMMENT 'Chave de isolamento.',
  `id_empresa` bigint unsigned NOT NULL,
  `codigo` varchar(20) NOT NULL,
  `expressao_regular` varchar(150) NOT NULL DEFAULT '',
  `is_valid` tinyint(1) NOT NULL DEFAULT 1 COMMENT '1 = válido; 0 = inválido',
  PRIMARY KEY (`id`),
  KEY `codigos_registro_descricao_tenants_FK` (`id_tenant`),
  KEY `codigos_registro_descricao_empresas_FK` (`id_empresa`),
  CONSTRAINT `codigos_registro_descricao_empresas_FK` FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `codigos_registro_descricao_tenants_FK` FOREIGN KEY (`id_tenant`) REFERENCES `tenants` (`id_tenant`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
