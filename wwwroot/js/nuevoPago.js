
document.addEventListener('DOMContentLoaded', (event) => {
    const dateInput = document.getElementById('Fecha');
    const today = new Date().toISOString().split('T')[0];
    dateInput.value = today;
});

//--------------------------
document.addEventListener('DOMContentLoaded', function () {
    var paymentModal = document.getElementById('paymentModal');
    paymentModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget; // Botón que activó el modal
        var itemId = button.getAttribute('data-id'); // Extraer información del atributo data-id
        var modalBodyInput = paymentModal.querySelector('.modal-body input[name="Id"]');
        modalBodyInput.value = itemId; // Asignar el id al campo oculto del modal
    });
});

$(document).ready(function () {
    $('#paymentModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Botón que activó el modal
        var itemId = button.data('id'); // Extraer información del atributo data-id
        var $modal = $(this);
        var $modalBodyInput = $modal.find('input[name="Id"]');
        var $contratoSelect = $modal.find('#Contrato_Id');

        // Asignar el id al campo oculto del modal
        $modalBodyInput.val(itemId);

        if (itemId) {
            // Solicitud AJAX para obtener el contrato específico
            $.ajax({
                url: '/Contrato/RepositorioContrato/ObtenerContrato/' + itemId,
                method: 'GET',
                success: function (data) {
                    // Establecer el contrato en los campos del modal
                    $contratoSelect.val(data.Contrato.Id);
                },
                error: function (xhr, status, error) {
                    console.error('Error:', error);
                }
            });
        } else {
            // Solicitud AJAX para obtener todos los contratos si no hay un ID
            $.ajax({
                url: '/Contrato/RepositorioContrato/ObtenerTodo',
                method: 'GET',
                success: function (data) {
                    $contratoSelect.empty(); // Limpiar opciones actuales
                    // Agregar las opciones al select
                    $.each(data, function (index, contrato) {
                        $contratoSelect.append(
                            $('<option></option>').val(contrato.Id).text(contrato.Direccion)
                        );
                    });
                },
                error: function (xhr, status, error) {
                    console.error('Error:', error);
                }
            });
        }
    });
});
