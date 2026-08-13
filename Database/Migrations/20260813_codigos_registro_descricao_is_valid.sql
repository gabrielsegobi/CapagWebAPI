-- Flag de validade do código de registro descrição

ALTER TABLE `codigos_registro_descricao`
    ADD COLUMN `is_valid` TINYINT(1) NOT NULL DEFAULT 1
        COMMENT '1 = válido; 0 = inválido'
        AFTER `expressao_regular`;
