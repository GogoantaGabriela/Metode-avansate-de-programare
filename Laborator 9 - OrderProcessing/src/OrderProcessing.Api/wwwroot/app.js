const state = { orders: [], selectedId: null };

const PRODUCTS = [
  { id: '00000000-0000-0000-0000-000000000001', name: 'Produs A',        price: 49.99,  age: false },
  { id: '00000000-0000-0000-0000-000000000002', name: 'Produs B',        price: 129.50, age: false },
  { id: '00000000-0000-0000-0000-000000000003', name: 'Produs C (0 stoc)',price: 19.99, age: false },
  { id: '00000000-0000-0000-0000-000000000004', name: 'Produs D',        price: 299.00, age: false },
  { id: '00000000-0000-0000-0000-000000000005', name: 'Produs E (18+)',  price: 89.00,  age: true  },
];

const ALLOWED = {
  Pending:    { pay:true,  process:false, ship:false, deliver:false, cancel:true  },
  Confirmed:  { pay:false, process:true,  ship:false, deliver:false, cancel:true  },
  Processing: { pay:false, process:false, ship:true,  deliver:false, cancel:true  },
  Shipped:    { pay:false, process:false, ship:false, deliver:true,  cancel:false },
  Delivered:  { pay:false, process:false, ship:false, deliver:false, cancel:false },
  Cancelled:  { pay:false, process:false, ship:false, deliver:false, cancel:false },
};
const FLOW = ['Pending','Confirmed','Processing','Shipped','Delivered'];

async function fetchOrders() {
  const res = await fetch('/orders');
  state.orders = await res.json();
  renderList();
  if (state.selectedId) {
    const found = state.orders.find(o => o.id === state.selectedId);
    if (found) renderDetail(found);
  }
}

async function fetchOrder(id) {
  const res = await fetch(`/orders/${id}`);
  if (!res.ok) return;
  renderDetail(await res.json());
}

async function createOrder(data) {
  const res = await fetch('/orders', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
  });
  const json = await res.json();
  if (!res.ok) {
    showModalErrors(json.errors || [json.error || 'Eroare necunoscută']);
    return;
  }
  closeModal();
  state.selectedId = json.id;
  await fetchOrders();
  renderDetail(json);
  toast('success', `Comandă creată cu succes #${json.shortId}`);
}

async function triggerAction(id, action) {
  const res = await fetch(`/orders/${id}/${action}`, { method: 'POST' });
  const json = await res.json();
  if (res.status === 409) { toast('error', json.error); return; }
  if (!res.ok) { toast('error', json.error || 'Eroare necunoscută'); return; }
  renderDetail(json);
  await fetchOrders();
  toast('success', `${action.charAt(0).toUpperCase() + action.slice(1)} executat cu succes!`);
}

function renderList() {
  document.getElementById('order-count').textContent = `· ${state.orders.length}`;
  document.getElementById('order-list').innerHTML = state.orders.map(o => `
    <div class="order-row ${o.id === state.selectedId ? 'selected' : ''}"
         onclick="selectOrder('${o.id}')">
      <span class="marker">${o.id === state.selectedId ? '▶' : ''}</span>
      <span class="oid">#${o.shortId}</span>
      <span class="sbadge s-${o.status}">${o.status}</span>
    </div>`).join('');
}

function renderDetail(order) {
  state.selectedId = order.id;
  document.getElementById('detail-placeholder').style.display = 'none';
  const el = document.getElementById('order-detail');
  el.style.display = 'block';
  const a = ALLOWED[order.status] || {};

  const isCancelled = order.status === 'Cancelled';
  const currIdx = FLOW.indexOf(order.status);
  const flow = FLOW.map((s, i) => {
    const cls = s === order.status ? 'current' : (i < currIdx ? 'done' : '');
    return `<span class="sn ${cls}">${s}</span>${i < FLOW.length-1 ? '<span class="sa">→</span>' : ''}`;
  }).join('') + (isCancelled ? '<span class="sa"> /</span><span class="sn cancelled">Cancelled</span>' : '');

  const itemsHtml = order.items.map(i =>
    `<tr><td>${i.productName}</td><td>${i.quantity}</td>
     <td>${i.unitPrice.toFixed(2)} RON</td><td>${i.subtotal.toFixed(2)} RON</td></tr>`).join('');

  const histHtml = order.history.length === 0
    ? '<p style="color:var(--text3);font-size:13px">Nicio tranziție încă.</p>'
    : order.history.map(h =>
        `<div class="history-row"><span class="dot-h"></span>
         <span>${h.fromState} → ${h.toState}</span><span class="ts">${h.at}</span></div>`).join('');

  el.innerHTML = `
    <div class="detail-header">
      <h2 style="font-size:1.05rem;font-weight:700">Comandă</h2>
      <span class="sbadge s-${order.status}">${order.status}</span>
      <span class="detail-id">${order.id}</span>
    </div>
    <div class="detail-grid">
      <div class="detail-card">
        <div class="card-title">Client</div>
        <div class="card-row"><span>Nume</span><span>${order.customer.name}</span></div>
        <div class="card-row"><span>Email</span><span>${order.customer.email}</span></div>
        <div class="card-row"><span>Vârstă</span><span>${order.customer.age} ani</span></div>
        <div class="card-row"><span>Trusted</span><span>${order.customer.isTrusted ? '✓ Da' : '✗ Nu'}</span></div>
      </div>
      <div class="detail-card">
        <div class="card-title">Adresă livrare</div>
        <div class="card-row"><span>Stradă</span><span>${order.address.street}</span></div>
        <div class="card-row"><span>Oraș</span><span>${order.address.city}</span></div>
        <div class="card-row"><span>Cod poștal</span><span>${order.address.postalCode}</span></div>
        <div class="card-row"><span>Țară</span><span>${order.address.country}</span></div>
      </div>
    </div>
    <div class="detail-card" style="margin-bottom:1rem">
      <div class="card-title">State Diagram</div>
      <div class="state-flow">${flow}</div>
    </div>
    <div class="detail-card" style="margin-bottom:1rem">
      <div class="card-title">Acțiuni — tranziții permise din starea curentă</div>
      <div class="actions-row">
        <button class="action-btn ${a.pay?'enabled':''}" ${a.pay?'':'disabled'}
          onclick="triggerAction('${order.id}','pay')">Pay</button>
        <button class="action-btn ${a.process?'enabled':''}" ${a.process?'':'disabled'}
          onclick="triggerAction('${order.id}','process')">Process</button>
        <button class="action-btn ${a.ship?'enabled':''}" ${a.ship?'':'disabled'}
          onclick="triggerAction('${order.id}','ship')">Ship</button>
        <button class="action-btn ${a.deliver?'enabled':''}" ${a.deliver?'':'disabled'}
          onclick="triggerAction('${order.id}','deliver')">Deliver</button>
        <button class="action-btn ${a.cancel?'cancel-en':''}" ${a.cancel?'':'disabled'}
          onclick="triggerAction('${order.id}','cancel')">Cancel</button>
      </div>
    </div>
    <div class="detail-grid">
      <div class="detail-card">
        <div class="card-title">Produse</div>
        <table class="items-table">
          <thead><tr><th>Produs</th><th>Cant.</th><th>Preț</th><th>Subtotal</th></tr></thead>
          <tbody>${itemsHtml}</tbody>
        </table>
        <div class="total-row"><span>Total:</span><span>${order.total.toFixed(2)} ${order.currency}</span></div>
      </div>
      <div class="detail-card">
        <div class="card-title">History</div>
        ${histHtml}
      </div>
    </div>`;
  renderList();
}

function selectOrder(id) { state.selectedId = id; fetchOrder(id); }

let itemCount = 0;

function openModal() {
  document.getElementById('modal-overlay').style.display = 'flex';
  document.getElementById('items-list').innerHTML = '';
  document.getElementById('modal-errors').style.display = 'none';
  itemCount = 0;
  addItem();
}
function closeModal() { document.getElementById('modal-overlay').style.display = 'none'; }

function addItem() {
  const id   = itemCount++;
  const opts = PRODUCTS.map(p =>
    `<option value="${p.id}" data-price="${p.price}" data-age="${p.age}">${p.name}</option>`).join('');
  const div  = document.createElement('div');
  div.className = 'item-row'; div.id = `item-${id}`;
  div.innerHTML = `
    <select onchange="syncItem(${id})">${opts}</select>
    <input type="number" id="qty-${id}" value="1" min="1">
    <input type="number" id="price-${id}" value="${PRODUCTS[0].price}" step="0.01">
    <label><input type="checkbox" id="age-${id}" ${PRODUCTS[0].age?'checked':''}> 18+</label>
    <button class="btn-remove" onclick="document.getElementById('item-${id}').remove()">✕</button>`;
  document.getElementById('items-list').appendChild(div);
}

function syncItem(id) {
  const sel = document.querySelector(`#item-${id} select`);
  const opt = sel.options[sel.selectedIndex];
  document.getElementById(`price-${id}`).value = opt.dataset.price;
  document.getElementById(`age-${id}`).checked = opt.dataset.age === 'true';
}

function showModalErrors(errors) {
  const el = document.getElementById('modal-errors');
  el.style.display = 'block';
  el.innerHTML = errors.map(e => `<div>⚠ ${e}</div>`).join('');
}

async function submitOrder() {
  const rows = document.querySelectorAll('.item-row');
  if (!rows.length) { showModalErrors(['Adaugă cel puțin un produs.']); return; }
  const items = Array.from(rows).map(row => {
    const sel   = row.querySelector('select');
    const nums  = row.querySelectorAll('input[type=number]');
    return {
      productId: sel.value, productName: sel.options[sel.selectedIndex].text,
      quantity: parseInt(nums[0].value), unitPrice: parseFloat(nums[1].value),
      hasAgeRestriction: row.querySelector('input[type=checkbox]').checked
    };
  });
  await createOrder({
    customerName: document.getElementById('f-name').value,
    customerEmail: document.getElementById('f-email').value,
    customerAge: parseInt(document.getElementById('f-age').value),
    isTrusted: document.getElementById('f-trusted').checked,
    street: document.getElementById('f-street').value,
    city: document.getElementById('f-city').value,
    postalCode: document.getElementById('f-postal').value,
    items
  });
}

function toast(type, msg) {
  const c = document.getElementById('toast-container');
  const t = document.createElement('div');
  t.className = `toast ${type}`;
  t.innerHTML = `<span class="toast-msg">${msg}</span>
    <button class="toast-close" onclick="this.parentElement.remove()">✕</button>`;
  c.appendChild(t);
  setTimeout(() => t.remove(), 4000);
}

document.getElementById('btn-new-order').onclick = openModal;
fetchOrders();
