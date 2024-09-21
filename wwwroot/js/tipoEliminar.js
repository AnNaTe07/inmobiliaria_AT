/* document.addEventListener("DOMContentLoaded", function () {
  const btnRemove = document.getElementById("removeTipo");
  const modalEliminar = new bootstrap.Modal(
    document.getElementById("modal_eliminar_inmueble")
  );
  const modalMsg = document.getElementById("modal_message");
  const msgText = document.getElementById("message_text");
  const selectTipo = document.querySelector('select[name="TipoId"]');
  const hiddenTipoId = document.getElementById("inmueble_eliminar_id");

  if (btnRemove) {
    btnRemove.addEventListener("click", function () {
      const tipoId = selectTipo ? selectTipo.value : null;

      if (tipoId) {
        if (hiddenTipoId) {
          hiddenTipoId.value = tipoId; //valor del campo oculto
        }
        modalEliminar.show(); // Muestro el modal de eliminación
      } else {
        if (modalMsg) {
          msgText.textContent =
            "Por favor, selecciona un tipo de inmueble para eliminar.";
          const modalInstance = new bootstrap.Modal(modalMsg);
          modalInstance.show(); // Muestro el modal de mensaje de error

          // Oculto el modal
          setTimeout(() => {
            modalInstance.hide();
          }, 1000);
        } else {
          console.error("El modal para mensajes no está en el DOM.");
        }
      }
    });
  } else {
    console.error("Botón para eliminar no encontrado.");
  }
});
 */
