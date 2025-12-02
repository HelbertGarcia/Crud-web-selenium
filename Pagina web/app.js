//LÓGICA DE LOGIN 
const loginForm = document.getElementById('login-form');

if (loginForm) {
    loginForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const user = document.getElementById('username').value;
        const pass = document.getElementById('password').value;
        const errorMsg = document.getElementById('error-msg');

        if (user === 'admin' && pass === '1234') {
            localStorage.setItem('isLoggedIn', 'true');
            window.location.href = 'dashboard.html';
        } else {
            errorMsg.classList.remove('hidden');
        }
    });
}

function logout() {
    localStorage.removeItem('isLoggedIn');
    window.location.href = 'index.html';
}

//LÓGICA DE CRUD
const crudForm = document.getElementById('crud-form');
const tableBody = document.getElementById('table-body');
let editIndex = -1;

function loadProducts() {
    const products = JSON.parse(localStorage.getItem('products')) || [];
    tableBody.innerHTML = '';
    
    products.forEach((product, index) => {
        const row = `
            <tr>
                <td>${product.name}</td>
                <td>$${product.price}</td>
                <td>
                    <button class="edit" onclick="editProduct(${index})">Editar</button>
                    <button class="delete" onclick="deleteProduct(${index})">Eliminar</button>
                </td>
            </tr>
        `;
        tableBody.innerHTML += row;
    });
}

if (crudForm) {
    crudForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const name = document.getElementById('product-name').value;
        const price = document.getElementById('product-price').value;
        let products = JSON.parse(localStorage.getItem('products')) || [];

        if (editIndex === -1) {
            products.push({ name, price });
        } else {
            products[editIndex] = { name, price };
            editIndex = -1;
            document.getElementById('submit-btn').innerText = 'Guardar Producto';
            document.getElementById('submit-btn').style.backgroundColor = '#28a745';
        }

        localStorage.setItem('products', JSON.stringify(products));
        crudForm.reset();
        loadProducts();
    });

    document.getElementById('clear-btn').addEventListener('click', () => {
        crudForm.reset();
        editIndex = -1;
        document.getElementById('submit-btn').innerText = 'Guardar Producto';
    });
}

window.editProduct = (index) => {
    const products = JSON.parse(localStorage.getItem('products')) || [];
    document.getElementById('product-name').value = products[index].name;
    document.getElementById('product-price').value = products[index].price;
    editIndex = index;
    const btn = document.getElementById('submit-btn');
    btn.innerText = 'Actualizar';
    btn.style.backgroundColor = '#ffc107'; 
};

window.deleteProduct = (index) => {
    if(confirm('¿Estás seguro de eliminar este producto?')) {
        let products = JSON.parse(localStorage.getItem('products')) || [];
        products.splice(index, 1);
        localStorage.setItem('products', JSON.stringify(products));
        loadProducts();
    }
};
