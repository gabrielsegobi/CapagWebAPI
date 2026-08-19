-- Adiciona mensagem opcional em valores anuais (ex.: ROE com PL negativo).
ALTER TABLE valores_anuais
    ADD COLUMN mensagem VARCHAR(255) NULL AFTER valores_calc_ano;
