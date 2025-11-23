import {Home} from "@components/Home.tsx";
import Login from "@components/Login.tsx";
import { Routes, Route } from "react-router-dom";
import UserDashboard from "@components/sections/userSection/UserDashboard.tsx";
import {UserBoards} from "@components/sections/userSection/UserBoards.tsx";
import {GameBoard} from "@components/sections/userSection/GameBoard.tsx";

function App() {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/user/dashboard" element={<UserDashboard />} />
            <Route path="/user/my-boards" element={<UserBoards />} />
            <Route path="/game/current" element={<GameBoard/>} />

        </Routes>
    )
}

export default App
