
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



    document.addEventListener('DOMContentLoaded', function () {
        var paymentModal = document.getElementById('paymentModal');
        paymentModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget; // Botón que activó el modal
            var itemId = button.getAttribute('data-id'); // Extraer información del atributo data-id
            var modalBodyInput = paymentModal.querySelector('.modal-body input[name="Id"]');
            modalBodyInput.value = itemId; // Asignar el id al campo oculto del modal

            // Hacer la solicitud AJAX para obtener los detalles del contrato
            fetch(`/Contrato/ObtenerContrato?id=${itemId}`)
                .then(response => response.json())
                .then(data => {
                    // Establecer los datos del contrato en los campos del modal
                    document.getElementById('Fecha').value = data.Fecha;
                    document.getElementById('Monto').value = data.Monto;
                    document.getElementById('Contrato_Id').value = data.Contrato.Id;
                    document.getElementById('Concepto_Id').value = data.Concepto.Id;
                    document.getElementById('Detalle').value = data.Detalle;
                })
                .catch(error => console.error('Error:', error));
        });
    });
