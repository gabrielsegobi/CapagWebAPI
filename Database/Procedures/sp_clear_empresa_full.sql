-- Wipe amplo dos dados da empresa (cálculos + simulação/ICP anterior/débitos).
-- Preserva: cadastro da empresa, regs fiscais (reg_*) e ECF (operation/ecf_*).
-- Preferível executar com a API parada (ou sem job em andamento para a empresa).

DROP PROCEDURE IF EXISTS `gsaas`.`sp_clear_empresa_full`;

DELIMITER $$

CREATE DEFINER=`gmaster`@`%` PROCEDURE `gsaas`.`sp_clear_empresa_full`(
    IN p_tenant INT,
    IN p_empresa INT
)
BEGIN
    DECLARE v_existe INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    SELECT COUNT(1)
      INTO v_existe
      FROM gsaas.empresas
     WHERE id_tenant = p_tenant
       AND id_empresa = p_empresa
       AND deleted_at IS NULL;

    IF v_existe = 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Empresa não encontrada para o tenant informado.';
    END IF;

    START TRANSACTION;

    -- Pipeline calculado (mesma ordem da sp_clear_empresa_recalc)
    DELETE va
      FROM gsaas.valores_anuais va
      INNER JOIN gsaas.indicadores i ON i.id_indicador = va.id_indicador
     WHERE i.id_tenant = p_tenant
       AND i.id_empresa = p_empresa;

    DELETE FROM gsaas.resultados_indices_icp
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.analises_icp
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.indicadores
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.demonstrativos_contabeis
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.regimes_tributarios
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.resultados_periodo
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.process_log
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    -- Inputs de simulação / variáveis / ICP anterior
    DELETE FROM gsaas.simulacao_intervalo
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.simulacao_calc
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.descricao_debitos
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.valor_calc_variavel
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    DELETE FROM gsaas.icp_anterior
     WHERE id_tenant = p_tenant AND id_empresa = p_empresa;

    UPDATE gsaas.empresas
       SET dados_processados = 0,
           updated_at = CURRENT_TIMESTAMP
     WHERE id_tenant = p_tenant
       AND id_empresa = p_empresa;

    COMMIT;

    SELECT CONCAT(
        'Wipe completo: dados da empresa ',
        p_empresa,
        ' (tenant ',
        p_tenant,
        ') foram limpos (cálculos + simulação). Regs fiscais e ECF preservados.'
    ) AS mensagem;
END$$

DELIMITER ;
