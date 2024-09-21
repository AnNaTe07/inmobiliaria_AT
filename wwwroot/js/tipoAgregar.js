/* document.addEventListener("DOMContentLoaded", function () {
  const botonAgregar = document.getElementById("addTipo");
  const formularioAgregar = document.getElementById("formAgregarTipo");

  // Modales
  const modalAgregar = new bootstrap.Modal(
    document.getElementById("modalAgregarTipo")
  );

  // Mostrar el modal de agregar tipo al hacer clic en el botón
  if (botonAgregar) {
    botonAgregar.addEventListener("click", function () {
      modalAgregar.show();
    });
  }

  // Manejar el envío del formulario
  if (formularioAgregar) {
    formularioAgregar.addEventListener("submit", function (evento) {
      evento.preventDefault(); // Prevenir el envío tradicional del formulario

      // Obtener el valor del campo de descripción
      const descripcion = document.getElementById("nuevoTipoDescripcion").value;

      fetch(formularioAgregar.action, {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        body: new URLSearchParams({ descripcion: descripcion }),
      })
        .then((respuesta) => {
          return respuesta
            .text()
            .then((text) => ({ status: respuesta.status, text }));
        })
        .then(({ status, text }) => {
          if (status === 200) {
            // Mostrar mensaje de éxito
            document.getElementById("message_text").textContent =
              "Tipo agregado exitosamente";
            modalAgregar.hide(); // Ocultar el modal de agregar tipo
            setTimeout(() => location.reload(), 1000); // Recargar la página después de ocultar el modal
          } else if (status === 409) {
            // Mostrar mensaje de conflicto
            document.getElementById("message_text").textContent =
              text || "El tipo ya existe.";
            const modalError = new bootstrap.Modal(
              document.getElementById("modal_message")
            );
            modalError.show(); // Mostrar el modal

            // Ocultar el modal después de 1'
            setTimeout(() => {
              modalError.hide();
            }, 1000);
          } else {
            // Mostrar mensaje de error para otros códigos de error
            document.getElementById("message_text").textContent =
              text || "Error al agregar el tipo";
            const modalError = new bootstrap.Modal(
              document.getElementById("modal_message")
            );
            modalError.show(); // Mostrar el modal

            // Ocultar el modal después de 1'
            setTimeout(() => {
              modalError.hide();
            }, 1000);
          }
        })
        .catch((error) => {
          console.error("Error:", error);
          // Mostrar mensaje de error en caso de excepción
          document.getElementById("message_text").textContent =
            "Error al agregar el tipo";
          const modalError = new bootstrap.Modal(
            document.getElementById("modal_message")
          );
          modalError.show(); // Mostrar el modal de mensaje

          // Ocultar el modal después de 1'
          setTimeout(() => {
            modalError.hide();
          }, 1000);
        });
    });
  }
});
 */
