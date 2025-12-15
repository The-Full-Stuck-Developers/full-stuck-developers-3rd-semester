import { Navigate, Outlet } from "react-router-dom";
import { useAtom } from "jotai";
import { userInfoAtom } from "@components/../atoms/token";
import { loadable } from "jotai/utils";

const userInfoLoadableAtom = loadable(userInfoAtom);

// Protected Route for any authenticated user
export function ProtectedRoute() {
  const [user] = useAtom(userInfoLoadableAtom);

  if (user.state === "loading") {
    return <div>Loading...</div>;
  }

  if (user.state === "hasError" || !user.data) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}

// Admin Protected Route - only for admin users
export function AdminProtectedRoute() {
  const [user] = useAtom(userInfoLoadableAtom);

  if (user.state === "loading") {
    return <div>Loading...</div>;
  }

  if (user.state === "hasError" || !user.data) {
    return <Navigate to="/" replace />;
  }

  if (!user.data.isAdmin) {
    return <Navigate to="/player/dashboard" replace />;
  }

  return <Outlet />;
}

// User Protected Route - only for regular users (not admins)
export function UserProtectedRoute() {
  const [user] = useAtom(userInfoLoadableAtom);

  if (user.state === "loading") {
    return <div>Loading...</div>;
  }

  if (user.state === "hasError" || !user.data) {
    return <Navigate to="/" replace />;
  }

  if (user.data.isAdmin) {
    return <Navigate to="/admin/dashboard" replace />;
  }

  return <Outlet />;
}
