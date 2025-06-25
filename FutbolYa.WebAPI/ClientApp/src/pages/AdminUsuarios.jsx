import React, { useEffect, useState } from 'react';
import '../assets/styles/adminUsuarios.css';


const AdminUsuarios = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [nuevoUsuario, setNuevoUsuario] = useState({
    nombre: '',
    correo: '',
    contraseña: '',
    rol: 'jugador'
  });

  const token = localStorage.getItem('token');

  const cargarUsuarios = async () => {
    try {
      const res = await fetch('https://localhost:7055/api/usuarios', {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (!res.ok) throw new Error("Error al cargar usuarios");
      const data = await res.json();
      setUsuarios(data);
    } catch (error) {
      alert(error.message);
    }
  };

  const crearUsuario = async () => {
    const { nombre, correo, contraseña } = nuevoUsuario;

    if (!nombre || !correo || !contraseña) {
      alert('Por favor, completá todos los campos');
      return;
    }

    const res = await fetch('https://localhost:7055/api/usuarios', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(nuevoUsuario)
    });

    if (!res.ok) {
      const msg = await res.text();
      alert("Error: " + msg);
      return;
    }

    setNuevoUsuario({ nombre: '', correo: '', contraseña: '', rol: 'jugador' });
    cargarUsuarios();
  };

  const eliminarUsuario = async (id) => {
    if (!window.confirm("¿Estás seguro de eliminar este usuario?")) return;

    await fetch(`https://localhost:7055/api/usuarios/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token}` }
    });
    cargarUsuarios();
  };

  useEffect(() => {
    cargarUsuarios();
  }, []);

  return (
  <div className="admin-usuarios-container">
    <h2>Gestión de Usuarios</h2>

    <div className="admin-form">
      <input placeholder="Nombre" value={nuevoUsuario.nombre} onChange={e => setNuevoUsuario({ ...nuevoUsuario, nombre: e.target.value })} />
      <input placeholder="Correo" value={nuevoUsuario.correo} onChange={e => setNuevoUsuario({ ...nuevoUsuario, correo: e.target.value })} />
      <input placeholder="Contraseña" type="password" value={nuevoUsuario.contraseña} onChange={e => setNuevoUsuario({ ...nuevoUsuario, contraseña: e.target.value })} />
      <select value={nuevoUsuario.rol} onChange={e => setNuevoUsuario({ ...nuevoUsuario, rol: e.target.value })}>
        <option value="jugador">Jugador</option>
        <option value="establecimiento">Establecimiento</option>
        <option value="administrador">Administrador</option>
      </select>
      <button onClick={crearUsuario}>Crear</button>
    </div>

    <ul className="admin-usuarios-lista">
      {usuarios.map(u => (
        <li key={u.id}>
          {u.nombre} - {u.correo} - {u.rol}
          <button onClick={() => eliminarUsuario(u.id)}>Eliminar</button>
        </li>
      ))}
    </ul>
  </div>
);

};

export default AdminUsuarios;
