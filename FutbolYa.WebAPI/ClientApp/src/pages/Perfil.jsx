import React, { useEffect, useState } from 'react';
import '../assets/styles/perfil.css';

const Perfil = () => {
  const [jugador, setJugador] = useState(null);
  const [fotoCargando, setFotoCargando] = useState(false);

  useEffect(() => {
    const fetchJugador = async () => {
      try {
        const token = localStorage.getItem('token');

        const response = await fetch('https://localhost:7055/api/rendimientos/me', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!response.ok) throw new Error('No se pudo obtener el jugador');

        const data = await response.json();
        setJugador(data);
      } catch (error) {
        console.error('Error al obtener datos del jugador:', error);
      }
    };

    fetchJugador();
  }, []);

  const handleFotoChange = async (e) => {
    const archivo = e.target.files[0];
    if (!archivo) return;

    const formData = new FormData();
    formData.append('archivo', archivo);

    setFotoCargando(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch('https://localhost:7055/api/usuarios/subir-foto', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: formData,
      });

      if (!response.ok) throw new Error('Error al subir imagen');

      const data = await response.json();
      setJugador((prev) => ({ ...prev, fotoPerfil: data.ruta }));
    } catch (error) {
      console.error('Error al subir la foto:', error);
    } finally {
      setFotoCargando(false);
    }
  };

  if (!jugador) {
    return <div className="perfil-container">Cargando...</div>;
  }

  const imagenPerfil = jugador.fotoPerfil
    ? `https://localhost:7055${jugador.fotoPerfil}`
    : '/default-profile.png';

  return (
    <div className="perfil-container">
      <button className="volver-btn" onClick={() => window.location.href = '/Home'}>
        ⬅ Volver al Home
      </button>

      <h2 className="perfil-nombre">{jugador.nombre.toUpperCase()}</h2>

      <img src={imagenPerfil} alt="Foto del jugador" className="perfil-foto" />

      <div className="perfil-subir-foto">
        <label className="btn-subir-foto">
          Cambiar foto
          <input type="file" onChange={handleFotoChange} hidden />
        </label>
        {fotoCargando && <p>Subiendo imagen...</p>}
      </div>

      <ul className="perfil-info-lista">
        <li className="perfil-info-item">
          <img src="/pelotaIco.ico" alt="" />
          <div><strong>Partidos jugados</strong><p>{jugador.partidosJugados}</p></div>
        </li>
        <li className="perfil-info-item">
          <img src="/partidosGanados.ico" alt="" />
          <div><strong>Partidos ganados</strong><p>{jugador.partidosGanados}</p></div>
        </li>
        <li className="perfil-info-item">
          <img src="/partidosPerdidos.ico" alt="" />
          <div><strong>Partidos perdidos</strong><p>{jugador.partidosPerdidos}</p></div>
        </li>
        <li className="perfil-info-item">
          <img src="/PosicionFut.ico" alt="" />
          <div><strong>Posición de preferencia</strong><p>{jugador.posicion}</p></div>
        </li>
        <li className="perfil-info-item">
          <img src="/valoracion.ico" alt="" />
          <div><strong>Valoración promedio</strong><p>{jugador.valoracion ?? 'Sin evaluaciones'}</p></div>
        </li>
      </ul>

      <footer className="footer-perfil">© 2025 FútbolYa</footer>
    </div>
  );
};

export default Perfil;
