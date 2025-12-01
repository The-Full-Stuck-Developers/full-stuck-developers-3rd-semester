import {Home} from "@components/Home.tsx";
import Login from "@components/Login.tsx";
import { Routes, Route } from "react-router-dom";
import UserDashboard from "@components/sections/userSection/UserDashboard.tsx";
import {UserBoards} from "@components/sections/userSection/UserBoards.tsx";
import {GameBoard} from "@components/sections/userSection/GameBoard.tsx";
import UsersList from "@components/admin/UsersList.tsx";
import {SidebarLayout} from "@components/SidebarLayout.tsx";
import GameList from "@components/admin/GameList.tsx";
import "./../../node_modules/flag-icons/css/flag-icons.min.css";

function App() {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/user/dashboard" element={<UserDashboard />} />
            <Route path="/user/my-boards" element={<UserBoards />} />
            <Route path="/game/current" element={<GameBoard/>} />


            <Route path="/admin" element={<SidebarLayout />}>
                <Route path="users" element={<UsersList />} />
                <Route path="games" element={<GameList/>} />
            </Route>
        </Routes>
    )
}

export default App
