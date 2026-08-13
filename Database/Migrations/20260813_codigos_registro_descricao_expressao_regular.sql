-- Amplia expressao_regular de varchar(30) para varchar(150)

ALTER TABLE `codigos_registro_descricao`
    MODIFY COLUMN `expressao_regular` varchar(150) NOT NULL DEFAULT '';
