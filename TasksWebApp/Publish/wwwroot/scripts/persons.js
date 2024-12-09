(function () {
  let forms = document.querySelectorAll('.needs-validation')

  Array.prototype.slice.call(forms)
    .forEach(function (form) {
      form.addEventListener('submit', function (event) {
        if (!form.checkValidity()) {
          event.preventDefault()
          event.stopPropagation()
        }

        form.classList.add('was-validated')
      }, false)
    })

  var tab = document.getElementById("tab-mod-task")
  tab.addEventListener("show.bs.tab", function(event){
  let submitBtn = document.getElementById("submit-exception");
    
    if(event.target.id === "nav-add-tab") {
      submitBtn.setAttribute("formaction","/Persons/AddException");
      submitBtn.setAttribute("asp-page-handler","AddException")
      let textInput = document.getElementById("text-filter-task-add");
      textInput.textContent = "";
      submitBtn.dataset.color = "success"
      submitBtn.textContent = "Agregar"
    } else {
      submitBtn.setAttribute("formaction","/Persons/DeleteException");
      textInput = document.getElementById("text-filter-task-delete");
      textInput.textContent = "";
      submitBtn.setAttribute("asp-page-handler", "DeleteException")
      submitBtn.dataset.color = "danger"
      submitBtn.textContent = "Eliminar"
    }

    submitBtn.setAttribute("class", "btn btn-lg btn-outline-" + submitBtn.dataset.color);
  })
})()

function setDeleteModalData(button) {
    let divDel = document.getElementById("modal-delete-body");
    divDel.textContent = `¿Desea eliminar a ${button.dataset.name} de la base de datos?`;
    let buttonDel = document.getElementById("modal-delete-id");
    buttonDel.setAttribute("value", button.dataset.id);
}

function setAddTaskExceptData(button) {
    let submitBtn = document.getElementById("submit-exception");
    let inputTaskID = document.getElementById("except-task-id");
    let inputPersonID = document.getElementById("except-person-id");
    let textInput = document.getElementById("text-filter-task-add");
    textInput.textContent = "";
    textInput = document.getElementById("text-filter-task-delete");
    textInput.textContent = "";
  
    inputTaskID.setAttribute("value", "none");
    submitBtn.setAttribute("class", "btn btn-lg btn-outline-" + submitBtn.dataset.color);
    submitBtn.setAttribute("disabled", "true");
    
    let id = button.dataset.id;
    inputPersonID.setAttribute("value", id)
  
    let dboxAdd = document.getElementById("task-dropbox-menu-add");
    let dboxDelete = document.getElementById("task-dropbox-menu-delete");
    
    if(dboxAdd.children !== null)
    Array.from(dboxAdd.children).forEach( child => {
        dboxAdd.removeChild(child)
    })
    Array.from(dboxDelete.children).forEach( child => {
        dboxDelete.removeChild(child)
    })
  
    //petición a servidor 
    //-------------------v Handler (como asp-page-handler)
    fetch("/Persons/AvailTasks", {
        method: "POST",
        headers: {
            'RequestVerificationToken': document.getElementsByName("__RequestVerificationToken")[0].value,
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ data: [id] })
    })
    .then(response => response.json())
    .then(r => {
        if(r.success){

          r.data[0].forEach( task => {
              let li = document.createElement("li");
              let button = document.createElement("button");
              button.textContent = task.name;
              button.setAttribute("class", "dropdown-item");
              button.setAttribute("type", "button");
              button.dataset.id = task.id;
              
              button.onclick = function () {
                  let textInput = document.getElementById("text-filter-task-add");
                  textInput.value = this.textContent
                  let submit = document.getElementById("submit-exception");
                  submit.setAttribute("class", "btn btn-lg btn-success");
                  submit.removeAttribute("disabled");

                  let inputTaskId = document.getElementById("except-task-id")
                  inputTaskId.setAttribute("value", this.dataset.id);
              };

              li.appendChild(button);
              dboxAdd.appendChild(li)
          })

          r.data[1].forEach( task => {
            let li = document.createElement("li");
            let button = document.createElement("button");
            button.textContent = task.name;
            button.setAttribute("class", "dropdown-item");
            button.setAttribute("type", "button");
            button.dataset.id = task.id;
            
            button.onclick = function () {
                let textInput = document.getElementById("text-filter-task-delete");
                textInput.value = this.textContent
                let submit = document.getElementById("submit-exception");
                submit.setAttribute("class", "btn btn-lg btn-danger");
                submit.removeAttribute("disabled");

                let inputTaskId = document.getElementById("except-task-id")
                inputTaskId.setAttribute("value", this.dataset.id);
            };

            li.appendChild(button);
            dboxDelete.appendChild(li)
          })

        }
    })
  }

//función para listar tareas disponibles
function displayAvailTasks(textInput){

    let submitBtn = document.getElementById("submit-exception");
    let inputTaskID = document.getElementById("except-task-id")
    inputTaskID.setAttribute("value", "none")
    submitBtn.setAttribute("class", "btn btn-lg btn-outline-" + submitBtn.dataset.color);
    submitBtn.setAttribute("disabled", "true");

    let isAddTabSelected = submitBtn.dataset.color === "success";
    let dbox = isAddTabSelected ? document.getElementById("task-dropbox-menu-add") : document.getElementById("task-dropbox-menu-delete");
    let dboxOptions =  dbox.children;
    let dboxDiv = document.getElementById("dropdown-div");
    let filter = textInput.value.trim().toLowerCase();

    let triggerButton = document.createElement("button")
    triggerButton.style.display = "none";

    triggerButton.setAttribute("class", "btn btn-secondary dropdown-toggle")
    triggerButton.setAttribute("type", "button");
    triggerButton.setAttribute("aria-expanded", "false");
    triggerButton.setAttribute("data-bs-toggle", "dropdown");
    triggerButton.setAttribute("data-bs-auto-close", "true");
    dboxDiv.appendChild(triggerButton);
    if (dbox.style.display == "none")
        triggerButton.click();

    let countShown = 0;

    Array.from(dboxOptions).forEach( task => {
      let display;
        if(task.textContent.toLowerCase().includes(filter)) {
          display = "block";
          countShown++;
        } else {
          display = "none";
        }
        task.style.display = display;
    })
    
    if(!countShown) {
      let li = document.createElement("li");
      li.innerHTML=`<span class="dropdown-item-text">No hay tareas para ${isAddTabSelected ? "agregar" : "eliminar"}</span>`
      dbox.appendChild(li) 
    }
}