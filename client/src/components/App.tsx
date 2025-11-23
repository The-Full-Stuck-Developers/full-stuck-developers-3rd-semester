import {Home} from "@components/Home.tsx";
import Login from "@components/Login.tsx";
import { Routes, Route } from "react-router-dom";
import {GameBoard} from "@components/sections/GameBoard.tsx";

function App() {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
        </Routes>
    )
}

export default App
