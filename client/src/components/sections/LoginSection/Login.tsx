import { useState } from "react";
import { useNavigate } from "react-router";
import { type SubmitHandler, useForm } from "react-hook-form";
import type { LoginRequestDto } from "@core/generated-client.ts";
import toast, { Toaster } from "react-hot-toast";
import { useAuth } from "../../../hooks/auth.tsx";
import { authClient } from "../../../api-clients.ts";
import {getProblemTitle} from "@utilities/getProblemTitle.ts";

interface LoginModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function Login({ isOpen, onClose }: LoginModalProps) {
  // const [email, setEmail] = useState("");
  //const [password, setPassword] = useState("");
  const [showPass, setShowPass] = useState(false);
  //const [error, setError] = useState("");
  const [isForgotPassword, setIsForgotPassword] = useState(false);
  const [resetEmail, setResetEmail] = useState("");
  const navigate = useNavigate();
  const { login } = useAuth();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginRequestDto>();

  const onSubmit: SubmitHandler<LoginRequestDto> = async (data) => {
    try {
      await login(data);
      toast.success("Welcome back!");
      onClose();
    } catch (err: any) {
      console.log("LOGIN ERROR RAW:", err);
      const message =
          (await getProblemTitle(err)) ?? "Invalid email or password";
      toast.error(message);
    }
  };




  const handlePasswordReset = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await toast.promise(authClient.forgotPassword({ email: resetEmail }), {
        loading: "Sending reset email...",
        success: "If an account exists, you'll receive an email shortly",
        error: "Something went wrong. Please try again.",
      });
      setResetEmail("");
      setIsForgotPassword(false);
    } catch (error) {
      console.error("Password reset error:", error);
    }
  };
  const handleClose = () => {
    setIsForgotPassword(false);
    setResetEmail("");
    //setEmail("");
    //setPassword("");
    //setError("");
    onClose();
  };

  // Don't render if not open
  if (!isOpen) return null;

  return (
    <>
      <Toaster position="top-center" />

      {/* BACKDROP */}
      <div className="fixed inset-0 bg-black/65 z-40" onClick={handleClose} />

      {/* MODAL */}
      <div className="fixed inset-0 flex justify-center items-center z-50 px-6">
        <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md p-8 relative">
          {/* CLOSE BUTTON */}
          <button
            onClick={handleClose}
            className="absolute top-4 right-4 text-gray-400 hover:text-gray-600 transition"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>

          {/* FORGOT PASSWORD VIEW */}
          {isForgotPassword ? (
            <form
              onSubmit={handlePasswordReset}
              className="flex flex-col gap-6"
            >
              {/* BACK BUTTON */}
              <button
                type="button"
                onClick={() => setIsForgotPassword(false)}
                className="flex items-center gap-2 text-gray-600 hover:text-gray-800 transition w-fit"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-5 w-5"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth={2}
                    d="M15 19l-7-7 7-7"
                  />
                </svg>
                <span className="font-medium">Reset your password</span>
              </button>

              {/* HEADER */}
              <div className="text-center">
                <h2 className="text-3xl font-bold mb-2">
                  Forgot your password?
                </h2>
                <p className="text-gray-600">
                  You may receive an email after filing in this form.
                </p>
              </div>

              {/* EMAIL INPUT */}
              <div>
                <input
                  autoFocus={true}
                  type="email"
                  placeholder="Enter your email"
                  value={resetEmail}
                  onChange={(e) => setResetEmail(e.target.value)}
                  className="border rounded-xl px-4 py-3 text-lg w-full focus:outline-none focus:ring-2 focus:ring-red-700"
                  required
                />
              </div>

              <p className="text-sm text-gray-500 text-center">
                All fields marked with an * are required
              </p>

              {/* RECOVER BUTTON */}
              <button
                type="submit"
                className="bg-gray-800 hover:bg-gray-900 transition text-white py-4 rounded-xl text-lg font-semibold mt-4"
              >
                Recover password
              </button>
            </form>
          ) : (
            /* LOGIN VIEW */
            <form
              onSubmit={handleSubmit(onSubmit)}
              className="flex flex-col gap-5"
            >
              {/* HEADER */}
              <h1 className="text-4xl font-bold mb-2">Log in</h1>

              {/* EMAIL */}
              <div>
                <label className="font-medium block mb-2">Email</label>
                <input
                  autoFocus={true}
                  type="email"
                  placeholder="Enter email"
                  {...register("email", { required: "Email is required" })}
                  className="border rounded-xl px-4 py-3 text-lg w-full focus:outline-verde"
                />
                {errors.email && (
                  <p className="text-red-500 text-sm mt-1">
                    {errors.email.message}
                  </p>
                )}
              </div>

              {/* PASSWORD FIELD */}
              <div>
                <label className="font-medium block mb-2">Password</label>
                <div className="relative">
                  <input
                    type={showPass ? "text" : "password"}
                    placeholder="Enter password"
                    {...register("password", {
                      required: "Password is required",
                    })}
                    className="border rounded-xl px-4 py-3 text-lg w-full focus:outline-verde"
                  />
                  {errors.password && (
                    <p className="text-red-500 text-sm mt-1">
                      {errors.password.message}
                    </p>
                  )}
                  <button
                    type="button"
                    className="absolute right-3 top-3 text-gray-500"
                    onClick={() => setShowPass((p) => !p)}
                  >
                    👁
                  </button>
                </div>
              </div>

              {/* FORGOT PASS */}
              <button
                type="button"
                onClick={() => setIsForgotPassword(true)}
                className="text-sm underline font-medium hover:text-red-700 transition text-left"
              >
                Forgot password?
              </button>

              {/* LOGIN BUTTON */}
              <button
                type="submit"
                className="bg-red-700 hover:bg-red-800 transition text-white py-3 rounded-xl text-lg font-semibold"
              >
                Log in
              </button>
            </form>
          )}
        </div>
      </div>
    </>
  );
}
