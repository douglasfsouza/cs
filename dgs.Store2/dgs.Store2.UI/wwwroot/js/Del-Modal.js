let _id = 0;
let _prefixo = null;
let _url = null;

const myModal = new bootstrap.Modal(document.getElementById('excluirModal'), {
    keyboard: false
})


const myModalSenha = new bootstrap.Modal(document.getElementById('senhaModal'), {
    keyboard: false
})

function excluir(id, nome, prefixo, url) {
    
    const body = `${prefixo} ${nome}`;
    document.querySelector("#modalBody-desc").textContent = body;

    _id = id;
    _prefixo = prefixo;
    _url = url;

    myModal.show();
}

function confirmarDel() {
     
    fetch(`${_url}/${_id}`, { method: 'delete' })
        .then((response) => {
            myModal.hide();
            let msg = {error: false, msg:""}
            switch (response.status) {
                case 204:
                    msg.error = false;
                    msg.msg = `Exclusão ${_prefixo} com sucesso`;   
                    var linha = document.getElementById(`item-${_id}`);
                    linha.remove();
                    break;
                case 404:
                    msg.error = true;
                    msg.msg =`Registro ${_prefixo} Não encontrado`;
                    break;
                default:
                    msg.error = true;
                    msg.msg = `Erro na exclusão ${_prefixo}`;
            }

            

            let toastEl = document.querySelector('.toast');
            let toast = bootstrap.Toast.getOrCreateInstance(toastEl);
            toast.show();

            
            

             

        });
}
