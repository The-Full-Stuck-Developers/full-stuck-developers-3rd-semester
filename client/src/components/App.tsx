import {Home} from "@components/Home.tsx";
import {Routes, Route, Navigate} from "react-router-dom";
import UserDashboard from "@components/sections/userSection/UserDashboard.tsx";
import {UserBoards} from "@components/sections/userSection/UserBoards.tsx";
import {GameBoard} from "@components/sections/userSection/GameBoard.tsx";
import UsersList from "@components/admin/UsersList.tsx";
import {SidebarLayout} from "@components/SidebarLayout.tsx";
import ResetPassword from "@components/sections/LoginSection/ResetPassword.tsx";
import GameList from "@components/admin/GameList.tsx";
import "./../../node_modules/flag-icons/css/flag-icons.min.css";
import MyProfilePage from "@components/admin/MyProfilePage.tsx";
import {UserDeposit} from "@components/sections/userSection/UserDeposit.tsx";
import {AdminProtectedRoute, UserProtectedRoute} from "@components/routes/auth/ProtectedRoutes.tsx";
import TransactionsList from "@components/admin/TransactionsList.tsx";
import UserTransactionsList from "@components/admin/UserTransactionsList.tsx";
import Dashboard from "./admin/Dashboard";

function App() {
    return (
        <Routes>
            {/* Public routes */}
            <Route path="/" element={<Home/>}/>
            <Route path="/reset-password" element={<ResetPassword/>}/>

            {/* User-protected routes - only for authenticated non-admin users */}
            <Route element={<UserProtectedRoute/>}>
                <Route path="/user/dashboard" element={<UserDashboard/>}/>
                <Route path="/user/my-boards" element={<UserBoards/>}/>
                <Route path="/user/deposit" element={<UserDeposit userId="1d49a350-b52a-4c2a-aa4b-8a3d5f04c6aa"/>}/>
                <Route path="/game/current" element={<GameBoard/>}/>
            </Route>

            {/* Admin protected routes - only for authenticated admin users */}
            {/*<Route element={<AdminProtectedRoute />}>*/}
            <Route path="/admin" element={<SidebarLayout/>}>
                <Route path="dashboard" element={<Dashboard/>}/>
                <Route path="users" element={<UsersList/>}/>
                <Route path="games" element={<GameList/>}/>
                <Route path="my-profile" element={<MyProfilePage/>}/>
                <Route path="transactions" element={<TransactionsList/>}/>
                <Route path="transactions/user/:userId" element={<UserTransactionsList/>}/>
            </Route>
            {/*</Route>*/}

            {/* Catch all - redirect to home */}
            <Route path="*" element={<Navigate to="/" replace/>}/>
        </Routes>
    )
}

export default App
