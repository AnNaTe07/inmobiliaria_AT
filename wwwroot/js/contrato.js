

$(document).ready(function () {
    // Muestro el modal de exito si existe un mensaje en TempData
    if ($('#modal_success_message').length) {
        $('#modal_success_message').modal('show');
        // Oculto el modal después de 3 segundos
        setTimeout(function () {
            $('#modal_success_message').modal('hide');
        }, 3000);
    }
});

function anular(id, direccion, propietario, inquilino, tipo, plazo) {

    document.querySelector("#contrato_anular_id").value = id;
    document.querySelector("#contrato_anular_direccion").innerText = "Dirección: " + direccion;
    document.querySelector("#contrato_anular_propietario").innerText = "Propietario: " + propietario;
    document.querySelector("#contrato_anular_inquilino").innerText = "Inquilino: " + inquilino;
    document.querySelector("#contrato_anular_tipo").innerText = "Tipo: " + tipo;
    document.querySelector("#contrato_anular_plazo").innerText = "Plazo: " + plazo;
    $("#modal_anular_contrato").modal("show");
}
function eliminar(id, direccion, propietario, inquilino, tipo, plazo) {
    document.querySelector("#contrato_eliminar_id").value = id;
    document.querySelector("#contrato_eliminar_direccion").innerText = "Dirección: " + direccion;
    document.querySelector("#contrato_eliminar_propietario").innerText = "Propietario: " + propietario;
    document.querySelector("#contrato_eliminar_inquilino").innerText = "Inquilino: " + inquilino;
    document.querySelector("#contrato_eliminar_tipo").innerText = "Tipo: " + tipo;
    document.querySelector("#contrato_eliminar_plazo").innerText = "Plazo: " + plazo;
    $("#modal_eliminar_contrato").modal("show");
}

function filtrar() {
    var tipoFiltro = document.getElementById("tipoFiltro").value;


    // Mostrar los filtros según la opción seleccionada
    if (tipoFiltro === "Propietario") {
        document.getElementById("filtroPropietario").style.display = "block";
    } else if (tipoFiltro === "Propiedad") {
        document.getElementById("filtroPropiedad").style.display = "block";
    }
}

$(document).ready(function () {
    // Inicializa los selects con Select2
    $('.select2').select2({
        theme: 'bootstrap-5',
        width: '100%',
        placeholder: 'Seleccionar opción',
        allowClear: true
    }).on('select2:open', function () {
        // Reaplicar el borde cuando se abra el select
        $('.select2-selection').css('border', '1px solid #1a1a1a');
    });
});

