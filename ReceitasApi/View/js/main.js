const apiUrl = "http://localhost:5187/api/receitas"; // ajustar aqui se a porta mudar

function getIdFromUrl() {
    const params = new URLSearchParams(window.location.search);
    return params.get('id');
}

async function carregarAlergias() {
    const ddl = document.getElementById('alergiaSelect');
    ddl.innerHTML = '<option value="">Nenhuma</option>';
    try {
        const response = await fetch(apiUrl + '/alergias');
        const alergias = await response.json();
        alergias.forEach(alergia => {
            ddl.innerHTML += `<option value="${alergia}">${alergia}</option>`;
        });
    } catch (e) {
        ddl.innerHTML += '<option disabled>Erro ao carregar alergias</option>';
    }
}

async function filtrarReceitasPorAlergia() {
    const alergia = document.getElementById('alergiaSelect').value;
    const div = document.getElementById('receitas');
    if (!alergia) {
        div.innerHTML = "";
        return;
    }
    const response = await fetch(apiUrl);
    const receitas = await response.json();
    let filtradas = receitas;
    if (alergia) {
        filtradas = receitas.filter(r =>
            r.alergias && r.alergias.some(a => a.nome.toLowerCase() === alergia.toLowerCase())
        );
    }
    mostrarReceitas(filtradas);
}

async function carregarReceitas() {
    const ddl = document.getElementById('receitaSelect');
    ddl.innerHTML = '<option value="">Selecione uma receita</option>';
    try {
        const response = await fetch(apiUrl);
        const receitas = await response.json();
        receitas.forEach(receita => {
            ddl.innerHTML += `<option value="${receita.id}">${receita.nome}</option>`;
        });
    } catch (e) {
        ddl.innerHTML += '<option disabled>Erro ao carregar alergias</option>';
    }
}

function mostrarReceitas(receitas) {
    const div = document.getElementById('receitas');
    if (!receitas.length) {
        div.innerHTML = "<p>Nenhuma receita encontrada.</p>";
        return;
    }
    div.innerHTML = receitas.map(r =>
        `<div class="receita">
            <h3>${r.nome}</h3>
            <p>${r.descricao}</p>
            <p><strong>Alergias:</strong> ${r.alergias.map(a => a.nome).join(', ')}</p>
        </div>`
    ).join('');
}

document.addEventListener('DOMContentLoaded', () => {
    carregarAlergias();
    carregarReceitas();
    document.getElementById('btnFiltrarAlergia').onclick = filtrarReceitasPorAlergia;
});
if (document.getElementById('tabela-receitas')) {
    fetch(apiUrl)
        .then(res => {
            if (!res.ok) {
                res.text().then(msg => {
                    console.error("Erro ao buscar receitas:", res.status, res.statusText, msg);
                });
                alert("Erro ao buscar receitas.");
                return [];
            }
            return res.json();
        })
        .then(data => {
            const tbody = document.querySelector('#tabela-receitas tbody');
            tbody.innerHTML = "";
            data.forEach(r => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${r.nome}</td>
                    <td>${r.descricao}</td>
                    <td>${r.ingredientes ? r.ingredientes.join(', ') : '-'}</td>
                    <td class="alergia">${r.alergias && r.alergias.length > 0 ? r.alergias.map(a => a.nome).join(', ') : '-'}</td>
                    <td>${r.modoPreparo || '-'}</td>
                    <td>R$${r.valor ?? '-'}</td>
                    <td>
                        <button class="btn-editar" data-id="${r.id}">Editar</button>
                        <button class="btn-remover" data-id="${r.id}">Remover</button>
                    </td>
                `;
                tbody.appendChild(tr);
            });

            document.querySelectorAll('.btn-remover').forEach(btn => {
                btn.onclick = async function () {
                    if (confirm("Tem certeza que deseja remover?")) {
                        const id = this.getAttribute('data-id');
                        try {
                            const res = await fetch(`${apiUrl}/${id}`, { method: "DELETE" });
                            if (res.ok) {
                                this.closest('tr').remove();
                            } else {
                                const errorMsg = await res.text();
                                console.error("Erro ao remover:", res.status, res.statusText, errorMsg);
                                alert("Erro ao remover: " + errorMsg);
                            }
                        } catch (err) {
                            console.error("Erro inesperado ao remover:", err);
                            alert("Erro inesperado ao remover.");
                        }
                    }
                };
            });

            document.querySelectorAll('.btn-editar').forEach(btn => {
                btn.onclick = function () {
                    const id = this.getAttribute('data-id');
                    window.location.href = `cadastro.html?id=${id}`;
                };
            });
        })
        .catch(err => {
            console.error("Erro ao carregar receitas (catch):", err);
            alert("Erro ao carregar receitas.");
        });
}

if (document.getElementById('form-receita')) {
    const id = getIdFromUrl();
    if (id) {
        fetch(`${apiUrl}/${id}`)
            .then(res => {
                if (!res.ok) {
                    res.text().then(msg => {
                        console.error("Erro ao buscar receita para edição:", res.status, res.statusText, msg);
                    });
                    document.getElementById("mensagem").textContent = "Erro ao buscar receita para edição.";
                    return null;
                }
                return res.json();
            })
            .then(receita => {
                if (!receita) return;
                const form = document.getElementById('form-receita');
                form.nome.value = receita.nome;
                form.descricao.value = receita.descricao;
                form.valor.value = receita.valor;
                form.ingredientes.value = (receita.ingredientes || []).join(', ');
                form.alergenos.value = (receita.alergias || []).map(a => a.nome).join(', ');
                form.modopreparo.value = receita.modoPreparo || '';
            })
            .catch(err => {
                console.error("Erro ao preencher formulário de edição (catch):", err);
                document.getElementById("mensagem").textContent = "Erro ao preencher formulário de edição.";
            });
    }

    document.getElementById("form-receita").onsubmit = async (e) => {
        e.preventDefault();
        const form = e.target;
        const receita = {
          nome: form.nome.value,
          descricao: form.descricao.value,
          modoPreparo: form.modopreparo.value,
          valor: parseFloat(form.valor.value.replace(",", ".")),
          ingredientes: form.ingredientes.value
            .split(",")
            .map((i) => i.trim())
            .filter((i) => i),
          alergias: form.alergenos.value
            .split(",")
            .map((a) => a.trim())
            .filter((a) => a)
            .map((a) => ({ nome: a })),
        };

        if (id) {
          receita.id = parseInt(id);
          try {
            const res = await fetch(`${apiUrl}/${id}`, {
              method: "PUT",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify(receita),
            });
            if (res.ok) {
              document.getElementById("mensagem").textContent =
                "Receita editada!";
              form.reset();
            } else {
              const errorMsg = await res.text();
              console.error(
                "Erro ao editar:",
                res.status,
                res.statusText,
                errorMsg
              );
              document.getElementById("mensagem").textContent =
                "Erro ao editar: " + (errorMsg || `Status ${res.status}`);
            }
          } catch (err) {
            console.error("Erro inesperado ao editar (catch):", err);
            document.getElementById("mensagem").textContent =
              "Erro inesperado ao editar.";
          }
        } else {
          try {
            const res = await fetch(apiUrl, {
              method: "POST",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify(receita),
            });
            if (res.ok) {
              document.getElementById("mensagem").textContent =
                "Receita cadastrada!";
              form.reset();
            } else {
              const errorMsg = await res.text();
              console.error(
                "Erro ao cadastrar:",
                res.status,
                res.statusText,
                errorMsg
              );
              document.getElementById("mensagem").textContent =
                "Erro ao cadastrar: " + (errorMsg || `Status ${res.status}`);
            }
          } catch (err) {
            console.error("Erro inesperado ao cadastrar (catch):", err);
            document.getElementById("mensagem").textContent =
              "Erro inesperado ao cadastrar.";
          }
        }
    };
}

if (document.getElementById('total-receitas')) {
    fetch(apiUrl)
        .then(res => {
            if (!res.ok) {
                res.text().then(msg => {
                    console.error("Erro ao buscar receitas para relatório:", res.status, res.statusText, msg);
                });
                document.getElementById("total-receitas").textContent = "Erro";
                return [];
            }
            return res.json();
        })
        .then(data => {
            const total = data.reduce((sum, r) => sum + (r.valor || 0), 0);
            document.getElementById('total-receitas').textContent = total.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

            const qtdAlergenos = data.filter(
                (r) => r.alergias && r.alergias.length > 0
            ).length;
            document.getElementById("qtd-alergenos").textContent = qtdAlergenos;

            const maisCaras = data
                .sort((a, b) => (b.valor || 0) - (a.valor || 0))
                .slice(0, 3);
            const ol = document.getElementById("mais-caras");
            ol.innerHTML = "";
            maisCaras.forEach((r) => {
                const li = document.createElement("li");
                li.textContent = `${r.nome} - ${r.valor?.toLocaleString("pt-BR", {
                    style: "currency",
                    currency: "BRL",
                })}`;
                ol.appendChild(li);
            });
        })
        .catch(err => {
            console.error("Erro ao carregar relatório (catch):", err);
            document.getElementById("total-receitas").textContent = "Erro";
        });
}