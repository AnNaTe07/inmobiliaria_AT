document.addEventListener("DOMContentLoaded", function () {
  // Selecciona el botón y los modales
  const btnEdit = document.querySelector(".btn-warning");
  const modalModificar = new bootstrap.Modal(
    document.getElementById("modalModificar")
  );
  const modalMessage = new bootstrap.Modal(
    document.getElementById("modal_message")
  );

  // Evento para el botón de editar
  if (btnEdit) {
    btnEdit.addEventListener("click", function () {
      const tipoSelect = document.getElementById("tipoSelect");

      if (tipoSelect && tipoSelect.value) {
        // Obtener el valor y el texto del tipo seleccionado
        const tipoId = tipoSelect.value;
        const tipoDescripcion =
          tipoSelect.options[tipoSelect.selectedIndex].text;

        // Cargar el valor seleccionado en el modal
        document.getElementById("tipoIdModificar").value = tipoId;
        document.getElementById("tipoDescripcionModificar").value =
          tipoDescripcion;

        // Mostrar el modal de modificar
        modalModificar.show();
      } else {
        // Mostrar el mensaje de error si no hay tipo seleccionado
        const messageText = document.getElementById("message_text");
        if (messageText) {
          messageText.textContent =
            "Por favor, selecciona un tipo de inmueble para modificar.";
          modalMessage.show();

          // Ocultar el modal después de 2 segundos
          setTimeout(() => {
            modalMessage.hide();
          }, 2000); // 2000 milisegundos
        } else {
          console.error("El elemento de mensaje no se encontró.");
        }
      }
    });
  } else {
    console.error("Botón no encontrado.");
  }
});
