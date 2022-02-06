document.querySelector("nav a").classList.remove('active');

let url = window.location.href;
let last = url.split("/")[url.split("/").length - 1]
if (!isNaN(last)) {
    last = url.split("/")[url.split("/").length - 2]
}
switch (last.toLowerCase()) {
    case "clientes":
    case "adicionar":
    case "editar":
    case "excluir":
        document.querySelector("#menuClientes").classList.add('active');

        break;
    case "about":
        document.querySelector("#menuAbout").classList.add('active');

        break;
    default:
        document.querySelector("#menuHome").classList.add('active');

}