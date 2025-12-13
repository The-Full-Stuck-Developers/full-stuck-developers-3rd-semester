import { Navigate, Outlet } from "react-router-dom";
import { useAtom } from "jotai";
import { userInfoAtom } from "@components/../atoms/token";

// Protected Route for any authenticated user
export function ProtectedRoute() {
  const [user] = useAtom(userInfoAtom);

  // If not authenticated, redirect to the homepage
  if (!user) {
    return <Navigate to="/" replace />;
  }

  // If authenticated, render the protected component
  return <Outlet />;
}

// Admin Protected Route - only for admin users
export function AdminProtectedRoute() {
  const [user] = useAtom(userInfoAtom);

  // If not authenticated, redirect to home
  if (!user) {
    return <Navigate to="/" replace />;
  }

  // If authenticated but not admin, redirect to the user dashboard
  if (!user.isAdmin) {
    return <Navigate to="/user/dashboard" replace />;
  }

  // If authenticated and admin, render the component
  return <Outlet />;
}

// User Protected Route - only for regular users (not admins)
export function UserProtectedRoute() {
  const [user] = useAtom(userInfoAtom);

  // If not authenticated, redirect to home
  if (!user) {
    return <Navigate to="/" replace />;
  }

  // If user is admin, redirect to admin dashboard
  if (user.isAdmin) {
    return <Navigate to="/admin/users" replace />;
  }

  // If authenticated and not admin, render the component
  return <Outlet />;
}
