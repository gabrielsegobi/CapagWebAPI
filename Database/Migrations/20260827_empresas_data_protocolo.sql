-- Data de protocolo na tabela empresas

ALTER TABLE `empresas`
  ADD COLUMN `data_protocolo` DATE NULL
    COMMENT 'Data de protocolo da empresa'
    AFTER `id_usuario_responsavel`;
