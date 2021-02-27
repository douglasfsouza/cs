var uri = "api/Despesas"

function Listar(pcodigo, texto) {
    
    let metodo;   
    if (pcodigo != 0) {        
        uri = "api/Despesas"
        uri += `/${pcodigo}`
        metodo = "get"        
        fetch(uri, {
            headers: { 'content-type': 'application/json' },
            method: metodo,
            mode: 'no-cors',
            Access: { 'Control-Allow-Origin': '*' }             
        })
            .then(response => _parseJSON(response))
            .then(data => listarDsp(data, pcodigo))
            .catch(error => console.error("Não foi possivel ler as despesas", error));
    }
    else {
        let criterios = []
        let eAno = document.getElementById("txtAno")
        let eMes = document.getElementById("selMes")
        let eTipo = document.getElementById("selTipo")
        if (texto == undefined) {
            uri = "api/Despesas/GetList"
            criterios.push({
                "Field": "Ano",
                "Operator": "eq",
                "Value": eAno.value
            })

            criterios.push({
                "Field": "Mes",
                "Operator": "eq",
                "Value": eMes.value
            })

            if (eTipo.value != "*") {
                criterios.push({
                    "Field": "Tipo",
                    "Operator": "eq",
                    "Value": eTipo.value
                })
            }
        }

        else {
            uri = "api/Despesas/GetListSearch"
            
            criterios.push({
                "Field": "Descricao",
                "Operator": "eq",
                "Value": texto
            })
        }     
        
        
        metodo = "POST"
        console.log(JSON.stringify(criterios))
        fetch(uri, {
            headers: {
                'Content-Type': 'application/json', 'Content-Encoding': 'utf-8', 'Accept-Encoding': '*'
            },
            method: metodo,            
            Access: { 'Control-Allow-Origin': '*' },
            body: JSON.stringify(criterios)
        })
            .then(response => _parseJSON(response))
            .then(data => listarDsp(data, pcodigo))
            .catch(error => console.error("Não foi possivel ler as despesas", error));
        carregarSummary("api/Despesas/GetListSummary", metodo,eAno, eMes, eTipo, criterios);
    }
}

function carregarSummary(pUri, pMetodo, pAno, pMes, pTipo, pCriterios) {
   
    fetch(pUri, {
        headers: {
            'Content-Type': 'application/json', 'Content-Encoding': 'utf-8', 'Accept-Encoding': '*'
        },
        method: pMetodo,
        Access: { 'Control-Allow-Origin': '*' },
        body: JSON.stringify(pCriterios)
    })
        .then(response => _parseJSON(response))
        .then(data => listarSummary(data))
        .catch(error => console.error("Não foi possivel ler os totais de despesas", error));

}

function listarSummary(data) {
    eTotDeb = document.getElementById("txtTotDeb");
    eTotCre = document.getElementById("txtTotCre");
    eTotTot = document.getElementById("txtTotTot");
    let tot = 0;

    data.forEach(function (d) {
        if (d.tipo.toLowerCase() == 'c') {
            eTotCre.value = d.valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });;
        }
        else {
            eTotDeb.value = d.valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });;
        }
        tot += d.valor;
    })

    eTotTot.value = tot.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
    
}

function listarDsp(data, pcodigo) {
    
    if (pcodigo != 0) {
        
        eData = document.getElementById("dtmData")
        eDescricao = document.getElementById("txtDescricao")
        eTipo = document.getElementById("cboTipo")
        eValor = document.getElementById("txtValor")        
                 
        let d = new Date(Date.parse(data.data))
        eData.value = `${d.getFullYear()}-${leftPad(d.getMonth() + 1,2)}-${leftPad(d.getDate(),2)}`
        
        eDescricao.value = data.descricao
               
        eTipo.value = data.tipo.toLowerCase()

        eValor.value = data.valor
        
    }
    else {
        const tBody = document.getElementById("todos")
        tBody.innerHTML = ''

        for (var i = 0; i < data.length; i++) {

            let eSel = document.createElement("input")
            eSel.type = "checkbox"
            eSel.id = data[i].id

            let eId = document.createElement("label")
            eId.type = "text"
            eId.innerText = data[i].id

            let eData = document.createElement("label")
            eData.type = "text"
            let d = new Date(Date.parse(data[i].data))

            eData.innerText = `${d.getDate()}-${d.getMonth() + 1}-${d.getFullYear()}`

            let eDescricao = document.createElement("label")
            eDescricao.type = "text"
            eDescricao.innerText = data[i].descricao

            let eTipo = document.createElement("label")
            eTipo.type = "text"
            let dTipo = "Indef"
            switch (data[i].tipo.toLowerCase()) {
                case "c":
                    dTipo = "Credito"
                    break
                case "d":
                    dTipo = "Debito"
                    break
            }
            eTipo.innerText = dTipo

            let eValor = document.createElement("label")
            eValor.type = "text"
            eValor.innerText = data[i].valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

            let tr = tBody.insertRow();

            let td1 = tr.insertCell(0)
            td1.appendChild(eSel)

            let td2 = tr.insertCell(1)
            td2.appendChild(eId)

            let td3 = tr.insertCell(2)
            td3.appendChild(eData)

            let td4 = tr.insertCell(3)
            td4.appendChild(eDescricao)

            let td5 = tr.insertCell(4)
            td5.appendChild(eTipo)

            let td6 = tr.insertCell(5)
            td6.appendChild(eValor)
        }
    }    
}

function Incluir() {    
    eData = document.getElementById("dtmData")
    eDescricao = document.getElementById("txtDescricao")
    eTipo = document.getElementById("cboTipo")
    eValor = document.getElementById("txtValor")
    let dTipo = "Indef"    
    
    let dados = {
                 "id": 0,
                 "data": eData.value, 
                 "descricao": eDescricao.value, 
        "tipo": eTipo.value,
        "valor": parseFloat( eValor.value)
                  }
    console.log(JSON.stringify(dados))
    fetch(uri, {
        headers: {
            'Content-Type': 'application/json', 'Content-Encoding':'utf-8', 'Accept-Encoding':'*' },
        method: 'POST',        
        Access: { 'Control-Allow-Origin': '*' },
        body: JSON.stringify(dados)
    })
        .then(response => response.json())
        .then(() => { alert("Dados incluidos") })
        .catch(error => {console.error('nao foi possivel incluir',error)})
}

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('unable to get items.', error));
}

function _displayItems(data) {
    const tBody = document.getElementById('todos');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let isCompleteCheckbox = document.createElement('input');
        isCompleteCheckbox.type = 'checkbox';
        isCompleteCheckbox.disabled = true;
        isCompleteCheckbox.checked = item.isComplete;

        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        td1.appendChild(isCompleteCheckbox);

        let td2 = tr.insertCell(1);
        let textNode = document.createTextNode(item.name);
        td2.appendChild(textNode);

        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });

    todos = data;
}

function _parseJSON(response) {
    return response.text().then(function (text) {
        return text ? JSON.parse(text) : {}
    })
}

function reqListener() {
    console.log(this.responseText);
}

function Gravar(pcodigo) {    
    eData = document.getElementById("dtmData")
    eDescricao = document.getElementById("txtDescricao")
    eTipo = document.getElementById("cboTipo")
    eValor = document.getElementById("txtValor")
    let dTipo = "Indef"

    let dados = {      
        "id": parseInt( pcodigo),
        "data": eData.value,
        "descricao": eDescricao.value,
        "tipo": eTipo.value,
        "valor": parseFloat(eValor.value)
    }
    console.log(JSON.stringify(dados))
    fetch(uri, {
        headers: {
            'Content-Type': 'application/json', 'Content-Encoding': 'utf-8', 'Accept-Encoding': '*'
        },
        method: 'PUT',
        Access: { 'Control-Allow-Origin': '*' },
        body: JSON.stringify(dados)
    })
        .then(response => _parseJSON(response))
        .then(() => { alert("Dados atualizados") })
        .catch(error => { console.error('nao foi possivel alterar', error) })
}

function leftPad(value, totalWidth, paddingChar) {
    var length = totalWidth - value.toString().length + 1;
    return Array(length).join(paddingChar || '0') + value;
};


