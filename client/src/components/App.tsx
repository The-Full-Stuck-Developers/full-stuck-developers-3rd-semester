import { Home } from "@components/Home.tsx";
import { Routes, Route, Navigate } from "react-router-dom";
import UserDashboard from "@components/sections/userSection/UserDashboard.tsx";
import { UserBoards } from "@components/sections/userSection/UserBoards.tsx";
import { GameBoard } from "@components/sections/userSection/GameBoard.tsx";
import UsersList from "@components/admin/UsersList.tsx";
import { SidebarLayout } from "@components/SidebarLayout.tsx";
import ResetPassword from "@components/sections/LoginSection/ResetPassword.tsx";
import "./../../node_modules/flag-icons/css/flag-icons.min.css";
import MyProfilePage from "@components/admin/MyProfilePage.tsx";
import { UserDeposit } from "@components/sections/userSection/UserDeposit.tsx";
import {
  AdminProtectedRoute,
  UserProtectedRoute,
} from "@components/routes/auth/ProtectedRoutes.tsx";
import TransactionsList from "@components/admin/TransactionsList.tsx";
import UserTransactionsList from "@components/admin/UserTransactionsList.tsx";
import Dashboard from "./admin/Dashboard";
import { DashboardOverview } from "@components/sections/userSection/DashboardOverview.tsx";
import { PlayerLayout } from "@components/sections/userSection/PlayerLayout.tsx";
import { GameHistory } from "@components/sections/userSection/GameHistory.tsx";
import UpcomingGamesList from "@components/admin/UpcomingGamesList.tsx";
import PastGamesList from "@components/admin/PastGamesList.tsx";
import GameDetails from "@components/admin/GameDetails.tsx";

function App() {
  return (
    <Routes>
      {/* Public routes */}
      <Route path="/" element={<Home />} />
      <Route path="/reset-password" element={<ResetPassword />} />

      {/* Admin protected routes - only for authenticated admin users */}
      <Route element={<AdminProtectedRoute />}>
        <Route path="/admin" element={<SidebarLayout />}>
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="users" element={<UsersList />} />
          <Route path="upcoming-games" element={<UpcomingGamesList />} />
          <Route path="past-games" element={<PastGamesList />} />
          <Route path="game-details/:id" element={<GameDetails />} />
          {/*<Route path="my-profile" element={<MyProfilePage/>}/>*/}
          <Route path="transactions" element={<TransactionsList />} />
          <Route
            path="transactions/user/:userId"
            element={<UserTransactionsList />}
          />
        </Route>
      </Route>

      <Route element={<UserProtectedRoute />}>
        <Route path="/user/dashboard" element={<UserDashboard />} />
        <Route path="/user/my-boards" element={<UserBoards />} />

        <Route path="/game/current" element={<GameBoard />} />

        <Route element={<PlayerLayout />}>
          <Route path="/player/dashboard" element={<DashboardOverview />} />
          <Route path="/player/boards" element={<UserBoards />} />
          <Route path="/player/deposit" element={<UserDeposit />} />
          <Route path="/player/history" element={<GameHistory />} />
          <Route path="/game/current" element={<GameBoard />} />
        </Route>
      </Route>

      {/* Catch all - redirect to home */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
