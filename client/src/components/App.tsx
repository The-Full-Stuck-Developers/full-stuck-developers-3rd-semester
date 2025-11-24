import {Home} from "@components/Home.tsx";
import Login from "@components/Login.tsx";
import { Routes, Route } from "react-router-dom";
import {GameBoard} from "@components/sections/GameBoard.tsx";
import UsersList from "@components/admin/UsersList.tsx";
import {SidebarLayout} from "@components/SidebarLayout.tsx";

function App() {
    return (
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />

            <Route path="/admin" element={<SidebarLayout />}>
                <Route path="users" element={<UsersList />} />
            </Route>
        </Routes>
    )
}

export default App
