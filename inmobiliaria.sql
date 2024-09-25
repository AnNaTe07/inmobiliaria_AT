-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 25-09-2024 a las 02:13:16
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `concepto`
--

CREATE TABLE `concepto` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `concepto`
--

INSERT INTO `concepto` (`Id`, `Nombre`) VALUES
(1, 'Mensualidad'),
(2, 'Multa'),
(3, 'Depósito'),
(4, 'Adelanto'),
(5, 'Renovación'),
(6, 'Intereses'),
(7, 'Compra'),
(8, 'Impuestos'),
(9, 'Diferencia');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `Id` int(11) NOT NULL,
  `Inqui` int(11) NOT NULL,
  `Inmu` int(11) NOT NULL,
  `Prop` int(11) NOT NULL,
  `FechaInicio` datetime NOT NULL,
  `FechaFin` datetime NOT NULL,
  `Estado` int(1) DEFAULT 1,
  `Pagos` int(11) DEFAULT NULL,
  `Observaciones` varchar(350) NOT NULL,
  `Descripcion` varchar(350) NOT NULL,
  `UsuarioCreacion` int(11) NOT NULL,
  `UsuarioAnulacion` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`Id`, `Inqui`, `Inmu`, `Prop`, `FechaInicio`, `FechaFin`, `Estado`, `Pagos`, `Observaciones`, `Descripcion`, `UsuarioCreacion`, `UsuarioAnulacion`) VALUES
(1, 3, 5, 2, '2024-09-18 00:00:00', '2024-09-27 00:00:00', 3, 2, 'Contrato anulado por rescisión en la fecha 22/09/2024 a 4 dias del vencimiento.', 'LOLOLO', 1, 1),
(2, 4, 6, 4, '2024-09-21 00:00:00', '2024-09-22 00:00:00', 2, 5, 'afdasfd', 'sdfsdf', 6, NULL),
(3, 2, 5, 2, '2024-10-18 00:00:00', '2024-10-27 00:00:00', 3, 1, 'Contrato anulado por rescisión en la fecha 24/09/2024 a 2 dias del vencimiento.', 'eed', 8, 7),
(4, 2, 5, 2, '2024-09-12 00:00:00', '2024-09-17 00:00:00', 2, 3, 'fgfg', 'fgfgf', 1, NULL),
(5, 6, 7, 3, '2024-09-22 00:00:00', '2024-12-13 00:00:00', 1, 4, 'nuevo  3', 'nuevo', 1, NULL),
(6, 3, 2, 2, '2024-09-06 00:00:00', '2024-09-10 00:00:00', 2, 8, '324', '23', 1, NULL),
(7, 4, 7, 3, '2024-09-01 00:00:00', '2024-09-20 00:00:00', 2, 1, 'sdf', 'df', 1, NULL),
(8, 1, 5, 2, '2024-08-31 00:00:00', '2024-09-03 00:00:00', 2, 3, 'asd', 'asdasd', 1, NULL),
(11, 2, 5, 2, '2025-09-28 00:00:00', '2026-02-26 00:00:00', 4, 2, 'sdfsdfdf ', 'ee', 1, NULL),
(12, 2, 5, 2, '2025-02-27 00:00:00', '2025-03-29 00:00:00', 1, 3, 'sdfsdfdf ', 'ee', 1, NULL),
(13, 3, 5, 2, '2026-05-15 00:00:00', '2026-09-26 00:00:00', 4, 12, 'sdsj', '33333', 1, NULL),
(15, 2, 4, 4, '2024-09-20 00:00:00', '2025-02-26 00:00:00', 3, 6, 'Contrato anulado por rescisión en la fecha 24/09/2024 a 154 dias del vencimiento.', 'd fds sd dsf', 1, 7),
(16, 2, 2, 2, '2024-09-28 00:00:00', '2026-02-20 00:00:00', 1, 17, 'Ultimo contrato', 'Ultimo', 1, NULL),
(18, 6, 4, 4, '2024-07-01 00:00:00', '2024-09-01 00:00:00', 1, 2, 'aa', 'pp', 7, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Uso` tinyint(4) DEFAULT NULL,
  `TipoId` int(10) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Latitud` double NOT NULL,
  `Longitud` double NOT NULL,
  `Precio` decimal(18,2) NOT NULL,
  `IdPropietario` int(11) NOT NULL,
  `Superficie` decimal(10,0) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`Id`, `Direccion`, `Uso`, `TipoId`, `Ambientes`, `Latitud`, `Longitud`, `Precio`, `IdPropietario`, `Superficie`, `estado`) VALUES
(1, 'Las Heras 222', 2, 3, 3, 111100, 222200, 2222220000.00, 2, 43200, 1),
(2, 'Comechingones 333', 1, 1, 2, 11.22, 22.11, 333333.10, 2, 542, 0),
(3, 'calle 098', 1, 1, 3, 25.1, 42.47, 444444.00, 3, 125, 1),
(4, 'mi casa 777', 2, 4, 4, 111.11, 333.33, 777777.00, 4, 321, 1),
(5, 'Casa Quinta ', 2, 5, 7, 11.11, 99.99, 999999.00, 2, 400, 1),
(6, 'aquí 232', 1, 1, 2, 33.03, -66.231, 2569874.00, 4, 300, 1),
(7, 'hdd 965', 1, 3, 3, 3333.65, 4556, 4567.00, 3, 234, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Documento` varchar(20) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`Id`, `Nombre`, `Apellido`, `Documento`, `Telefono`, `Email`) VALUES
(1, 'Profesor', 'Xavier', '235689745', '26658741235', 'profe@email.com'),
(2, 'Pepe', 'Le Pu', '45235698', '26648521478', 'lepu@email.com'),
(3, 'Doña', 'Cleotilde', '23455698', '26658742365', 'cleotilde@mail.com'),
(4, 'Gallo', 'Claudio', '452356987', '266547896', 'gallo@mail.com'),
(6, 'Ariel', 'Sirenita', '56231245', '266432145698', 'ariel@email.com'),
(12, 'Geronimo', 'Troya', '11111111', '56234587', 'ger@mail');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `Id` int(11) NOT NULL,
  `IdContrato` int(11) NOT NULL,
  `Fecha` datetime NOT NULL DEFAULT current_timestamp(),
  `Monto` decimal(10,0) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `usuPago` int(11) NOT NULL,
  `usuAnulacion` int(11) DEFAULT NULL,
  `fechaAnulacion` datetime DEFAULT NULL,
  `Detalle` varchar(300) DEFAULT NULL,
  `Nro` int(2) NOT NULL,
  `Concepto` tinyint(2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`Id`, `IdContrato`, `Fecha`, `Monto`, `Estado`, `usuPago`, `usuAnulacion`, `fechaAnulacion`, `Detalle`, `Nro`, `Concepto`) VALUES
(3, 3, '2024-09-13 00:00:00', 999999, 0, 1, 1, '2024-09-13 19:09:12', 'PEPEPEPEPEPEP\n - Pago editado por: Juan Pepe - 9/22/2024 4:27:33 PM', 1, 1),
(44, 2, '2024-09-21 20:29:57', 45345, 1, 1, NULL, NULL, 'fdgdfg', 2, 1),
(45, 2, '2024-09-21 00:00:00', 3242, 0, 1, 1, '2024-09-21 20:34:23', 'dssdfsdfd', 3, 1),
(46, 2, '2024-09-21 22:59:09', 2569874, 1, 1, NULL, NULL, 'dsfsdfsd', 4, 1),
(49, 1, '2024-09-21 23:13:29', 999999, 1, 1, NULL, NULL, 'sfd', 2, 1),
(50, 1, '2024-09-21 00:00:00', 29999, 1, 1, NULL, NULL, 'dsfsd - Pago editado pd - Pago editado por: Juan Pepe - 9/22/2024 4:26:34 PM', 3, 1),
(54, 1, '2024-09-21 23:16:06', 999999, 0, 1, 1, '2024-09-22 00:04:29', 'fghfgh', 4, 1),
(55, 1, '2024-09-21 23:16:10', 999999, 0, 1, 1, '2024-09-22 03:29:48', 'fghfghf', 5, 1),
(62, 1, '2024-09-22 00:15:49', 6000, 0, 1, 1, '2024-09-22 05:34:29', 'Multa por rescisión de contrato a 4 dias del vencimiento.', 0, 2),
(63, 3, '2024-09-22 00:17:00', 444444, 0, 1, 1, '2024-09-22 05:34:24', 'asdasdasd', 1, 1),
(64, 3, '2024-09-22 03:29:44', 999999, 0, 1, 1, '2024-09-22 04:28:25', 'sdas', 2, 1),
(65, 5, '2024-09-22 03:29:58', 4567, 1, 1, NULL, NULL, 'dsda', 1, 1),
(66, 12, '2024-09-22 04:19:05', 999999, 1, 1, NULL, NULL, 'zdfasf', 1, 1),
(67, 16, '2024-09-22 05:31:32', 333333, 1, 1, NULL, NULL, 'Primer pago', 1, 1),
(68, 16, '2024-09-22 05:33:40', 333333, 1, 1, NULL, NULL, 'segundo ', 2, 1),
(69, 13, '2024-09-22 16:11:57', 999999, 1, 1, NULL, NULL, 'sd', 1, 1),
(71, 5, '2024-09-22 17:19:07', 4567, 0, 1, 8, '2024-09-24 16:40:49', 'c', 2, 1),
(72, 15, '2024-09-24 12:16:36', 1555554, 1, 7, NULL, NULL, 'Multa por rescisión de contrato a 154 dias del vencimiento.', 0, 2),
(73, 3, '2024-09-24 12:25:27', 999999, 1, 7, NULL, NULL, 'Multa por rescisión de contrato a 2 dias del vencimiento.', 0, 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Documento` varchar(20) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Direccion` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`Id`, `Nombre`, `Apellido`, `Documento`, `Telefono`, `Email`, `Direccion`) VALUES
(1, 'Ratón', 'Perez', '12564578', '26645689784', 'raton@email.com', 'Su casa 123'),
(2, 'Esteban', 'quito', '45569878', '2664853975', 'esteban@mail.com', 'Lanus 456'),
(3, 'Cindy', 'Entes', '65984578', '2664358962', 'cindy@mail.com', 'Su casa 236'),
(4, 'Pepe', 'Grillo', '98542821', '2665874532', 'pepe@mail.com', 'arbol 999'),
(6, 'Maria', 'Chuzena', '12564552', '266587653289', 'chuzena@mail.com', 'choza 456');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo`
--

CREATE TABLE `tipo` (
  `Id` int(11) NOT NULL,
  `Descripcion` varchar(100) NOT NULL,
  `Activo` tinyint(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo`
--

INSERT INTO `tipo` (`Id`, `Descripcion`, `Activo`) VALUES
(1, 'Local', 1),
(2, 'Depósito', 1),
(3, 'Departamento', 1),
(4, 'Casa', 1),
(5, 'Quinta', 1),
(21, 'rere', 0),
(22, 'sasasa', 0),
(23, 'sarasa', 1),
(27, 'agregar 2', 1),
(28, 'Agregado 1', 0),
(60, 'caseron', 1),
(61, 'qa', 0),
(62, 'werto', 0),
(65, 'poipoi', 1),
(78, 'pap', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(255) NOT NULL,
  `Apellido` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Clave` varchar(255) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL,
  `Rol` tinyint(4) NOT NULL,
  `Salt` varchar(255) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`Id`, `Nombre`, `Apellido`, `Email`, `Clave`, `Avatar`, `Rol`, `Salt`, `Estado`) VALUES
(1, 'Juan', 'Pepe', 'juan@email.com', '+ZGcTVcHDFUuwbz2U1BA34QvoG/6VryK0F7p6cvJCAY=', '/images/avatars/3-57d67d3a-0785-4b72-89f8-063493d2fe13.jpg', 1, 'xnyfFmnmrU0xQkE7z4HVmA==', 1),
(5, 'Pepe', 'Luis', 'pepe@mail.com', 'dAhFK7adl9IWLMFuhHFKtiJ135Dk4HJXH6wR0PR5mPs=', '/images/avatars/9-6558fe4b-7a08-44e0-ac7e-ceab770bf478.jpg', 2, 'eGpibCnbarc4GjO7+J9ZjQ==', 1),
(6, 'Levi', 'Ackerman', 'levi@mail.com', '9S+VgeaUkcH6n5dAOblq1xUbvRVeX+QPpJEgApysSf8=', '/images/avatars/levi-6958f4bb-7404-41af-bac6-794578a4949a.png', 1, 'bqfoaO+qdsBeWPqEjqBRLw==', 1),
(7, 'Ariel', 'Luis', 'ariel@email.com', 'QmTkr47y8Cid5LNozJrbVEXqd/B88DxwGGBKZlLHrOI=', '/images/avatars/8-8df86419-027e-460e-8ea7-be5b0a7946aa.jpg', 2, 'cMa4qMA1AwCvAUXL8WB9Fg==', 1),
(8, 'Juana', 'La Loca', 'juana@email.com', 'JwMaYN0Gjajb0BN8mYHCJ56UCICDtVQvTtDFccgqfqk=', '/images/avatars/7-c9edf105-a414-4528-aa94-9cc7d8b16abc.jpg', 1, 'FZQ9ANPubPjlZFQ2pZVf/g==', 1),
(9, 'Josefa', 'Perez', 'josefa@email.com', 'YTV5I21GmtKwhJdRWhc0g9fECrHHw55gotM/gMvkbBk=', '/images/avatars/4-0df948a5-a258-4c23-ab99-be380b33babb.jpg', 2, 'x0No1QHYPixM8/BJm962CA==', 1),
(10, 'Pedro', 'Perez', 'pedro@email.com', 'mV1kbE13++oUTmprY0Og3v0sMwbTKhPfnGWW7weWbP0=', '/images/avatars/6-b4abe71e-861d-4339-a17c-07eeabd6423d.jpg', 1, 'w/ysqaSj96oG46/58Bv+jw==', 1),
(11, 'ji', 'ji', 'ji@hu', '2Fwdlxq5c/tMNWZ6LVIcWAMxadkX/hTr35g71P872Y0=', '', 2, 'mJJaJHEyjWS0Il12FYJibg==', 0),
(12, 'Ulises', 'Moya', 'ulises@mail.com', 'gfkOwNMUNowg+L+EVL50KBqoTEU0+0DjzCYMISKOj7U=', NULL, 2, 'uhaDvP3AhL2TZ6w8P7plGw==', 1);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `concepto`
--
ALTER TABLE `concepto`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InquilinoId` (`Inqui`),
  ADD KEY `InmuebleId` (`Inmu`),
  ADD KEY `PropietarioId` (`Prop`),
  ADD KEY `UsuarioCreacion` (`UsuarioCreacion`),
  ADD KEY `UsuarioAnulacion` (`UsuarioAnulacion`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdPropietario` (`IdPropietario`),
  ADD KEY `TipoId` (`TipoId`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `DNI` (`Documento`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `usuarioPago` (`usuPago`),
  ADD KEY `usuarioAnulacion` (`usuAnulacion`),
  ADD KEY `IdContrato` (`IdContrato`),
  ADD KEY `Concepto` (`Concepto`),
  ADD KEY `UC_Contrato_Numero` (`IdContrato`,`Nro`) USING BTREE;

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `DNI` (`Documento`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `tipo`
--
ALTER TABLE `tipo`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UC_Descripcion` (`Descripcion`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`),
  ADD KEY `Email_2` (`Email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `concepto`
--
ALTER TABLE `concepto`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=74;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de la tabla `tipo`
--
ALTER TABLE `tipo`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=79;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`Inqui`) REFERENCES `inquilino` (`Id`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`Inmu`) REFERENCES `inmueble` (`Id`),
  ADD CONSTRAINT `contrato_ibfk_3` FOREIGN KEY (`Prop`) REFERENCES `propietario` (`Id`),
  ADD CONSTRAINT `contrato_ibfk_4` FOREIGN KEY (`UsuarioAnulacion`) REFERENCES `usuario` (`Id`),
  ADD CONSTRAINT `contrato_ibfk_5` FOREIGN KEY (`UsuarioCreacion`) REFERENCES `usuario` (`Id`);

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`TipoId`) REFERENCES `tipo` (`Id`),
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`IdPropietario`) REFERENCES `propietario` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
