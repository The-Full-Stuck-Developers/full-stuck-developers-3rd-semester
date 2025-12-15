import { useEffect, useState } from "react";
import {
  type CreateUserDto,
  type UpdateUserDto,
  type UserDto,
  UsersClient,
} from "@core/generated-client.ts";
import { baseUrl } from "@core/baseUrl.ts";
import Pagination from "../Pagination";
import {
  Check,
  Cross,
  Mail,
  Phone,
  SquarePen,
  TicketCheck,
  Trash2,
  UserPlus,
  UserX,
  Wallet,
  X,
} from "lucide-react";
import { ActionMenu } from "@components/ActionMenu.tsx";
import toast, { Toaster } from "react-hot-toast";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import getUserClient from "@core/clients/userClient.ts";

export default function UsersList() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [users, setUsers] = useState<UserDto[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  // Filter states
  const [emailFilter, setEmailFilter] = useState("");
  const [nameFilter, setNameFilter] = useState("");
  const [phoneFilter, setPhoneFilter] = useState("");
  const [isAdminFilter, setIsAdminFilter] = useState(false);
  const [pageSize, setPageSize] = useState(10);

  // Edit modal states
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);
  const [editForm, setEditForm] = useState({
    name: "",
    email: "",
    phoneNumber: "",
  });
  const [editErrors, setEditErrors] = useState<Record<string, string[]>>({});

  // Create modal states
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [createForm, setCreateForm] = useState({
    name: "",
    email: "",
    phoneNumber: "",
    password: "",
    isAdmin: false,
    activateMembership: false,
  });
  const [createErrors, setCreateErrors] = useState<Record<string, string[]>>(
    {},
  );

  const buildFilterString = () => {
    const filters: string[] = [];

    if (emailFilter) {
      filters.push(`Email@=*${emailFilter}`);
    }
    if (nameFilter) {
      filters.push(`Name@=*${nameFilter}`);
    }
    if (phoneFilter) {
      filters.push(`PhoneNumber@=*${phoneFilter}`);
    }

    if (isAdminFilter) {
      filters.push(`IsAdmin==true`);
    }

    return filters.length > 0 ? filters.join(",") : null;
  };

  const fetchUsers = (page: number) => {
    const client = getUserClient();
    const filterString = buildFilterString();

    client
      .getAllUsers(filterString, null, page, pageSize)
      .then((res) => {
        setUsers(res.items);
        setTotalPages(Math.ceil(res.total / pageSize));
      })
      .catch(console.error);
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      setCurrentPage(1);
      fetchUsers(1);
    }, 500);

    return () => clearTimeout(timer);
  }, [emailFilter, nameFilter, phoneFilter, pageSize, isAdminFilter]);

  useEffect(() => {
    fetchUsers(currentPage);
  }, [currentPage]);

  const handleDelete = (user: UserDto) => {
    const client = getUserClient();

    client
      .deleteUser(user.id!)
      .then(() => fetchUsers(currentPage))
      .catch(console.error);
  };

  const handleEdit = (user: UserDto) => {
    setSelectedUser(user);
    setEditForm({
      name: user.name || "",
      email: user.email || "",
      phoneNumber: user.phoneNumber || "",
    });
    setIsEditModalOpen(true);
  };

  const handleCloseEditModal = () => {
    setIsEditModalOpen(false);
    setSelectedUser(null);
    setEditForm({
      name: "",
      email: "",
      phoneNumber: "",
    });
  };

  const handleUpdateUser = async (e: React.MouseEvent) => {
    e.preventDefault();

    if (!selectedUser?.id) return;

    const client = getUserClient();
    const updateDto: UpdateUserDto = {
      name: editForm.name,
      email: editForm.email,
      phoneNumber: editForm.phoneNumber,
    };

    try {
      await client.updateUser(selectedUser.id, updateDto);
      fetchUsers(currentPage);
      handleCloseEditModal();
      setEditErrors({});
      toast.success("User updated successfully.");
    } catch (err: any) {
      if (err.status === 400) {
        const validation = err.errors;
        const formatted: Record<string, string[]> = {};

        Object.keys(validation).forEach((key) => {
          formatted[key] = validation[key];
        });

        setEditErrors(formatted);
      } else {
        toast.error("Something went wrong, please try again later.");
      }
    }
  };

  const handleOpenCreateModal = () => {
    setIsCreateModalOpen(true);
  };

  const handleCloseCreateModal = () => {
    setIsCreateModalOpen(false);
    setCreateForm({
      name: "",
      email: "",
      phoneNumber: "",
      password: "",
      isAdmin: false,
      activateMembership: false,
    });
    setCreateErrors({});
  };

  const handleCreateUser = async (e: React.MouseEvent) => {
    e.preventDefault();

    const client = getUserClient();
    const createDto: CreateUserDto = {
      name: createForm.name,
      email: createForm.email,
      phoneNumber: createForm.phoneNumber,
      isAdmin: createForm.isAdmin,
      activateMembership: createForm.activateMembership || false,
    };

    try {
      await client.createUser(createDto);
      fetchUsers(currentPage);
      handleCloseCreateModal();
      toast.success("User created successfully.");
      setCreateErrors({});
    } catch (err: any) {
      if (err.status === 400) {
        const validation = err.errors;
        const formatted: Record<string, string[]> = {};

        Object.keys(validation).forEach((key) => {
          formatted[key] = validation[key];
        });

        setCreateErrors(formatted);
      } else {
        toast.error("Something went wrong, please try again later.");
      }
    }
  };

  const handleRenewMembership = (user: UserDto) => {
    const client = getUserClient();

    client
      .renewMembership(user.id!)
      .then(() => {
        fetchUsers(currentPage);
        toast.success("Membership renewed successfully.");
        return;
      })
      .catch((error) => {
        try {
          const errorData = JSON.parse(error.response);
          toast.error(errorData.message);
          return;
        } catch {
          toast.error(error.message || "An error occurred");
          return;
        }
      });
  };

  const handleActivateUser = (user: UserDto) => {
    const client = getUserClient();

    client
      .activateUser(user.id!)
      .then(() => {
        fetchUsers(currentPage);
        toast.success("User activated successfully.");
        return;
      })
      .catch((error) => {
        const errorData = JSON.parse(error.response);
        toast.error(errorData.message);
      });
  };

  const handleDeactivateUser = (user: UserDto) => {
    const client = getUserClient();

    client
      .deactivateUser(user.id!)
      .then(() => {
        fetchUsers(currentPage);
        toast.success("User activated successfully.");
        return;
      })
      .catch((error) => {
        const errorData = JSON.parse(error.response);
        toast.error(errorData.message);
      });
  };

  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape" && (isEditModalOpen || isCreateModalOpen)) {
        handleCloseCreateModal();
        handleCloseEditModal();
      }
    };

    document.addEventListener("keydown", handleEscape);
    return () => document.removeEventListener("keydown", handleEscape);
  }, [isEditModalOpen, isCreateModalOpen]);

  return (
    <div>
      <Toaster position="top-center" />
      <div className={"flex flex-row items-center justify-between w-full pb-8"}>
        <p className={"text-white text-3xl mb-2 mx-0 p-0"}>{t("users")}</p>
        <button
          onClick={handleOpenCreateModal}
          className={
            "flex flex-row items-center justify-evenly px-3 py-1.5 rounded-lg bg-[#e30613] hover:bg-[#c20510] text-white shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer"
          }
        >
          <UserPlus size={18} className={"me-2"} />
          <span>{t("user:create_new_user")}</span>
        </button>
      </div>
      <div
        className={
          "flex flex-row justify-between bg-gray-800 rounded-2xl p-6 border border-gray-700 mb-6"
        }
      >
        <div className={"flex flex-row gap-4"}>
          <div className={""}>
            <input
              type="email"
              placeholder={t("placeholders:enter_email")}
              value={emailFilter}
              required={true}
              onChange={(e) => setEmailFilter(e.target.value)}
              className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
            />
          </div>
          <div>
            <input
              type="text"
              placeholder={t("placeholders:enter_name")}
              value={nameFilter}
              required={true}
              onChange={(e) => setNameFilter(e.target.value)}
              className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
            />
          </div>
          <div>
            <input
              type="text"
              placeholder={t("placeholders:enter_phone")}
              value={phoneFilter}
              required={true}
              onChange={(e) => setPhoneFilter(e.target.value)}
              className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
            />
          </div>
          <div className={"flex flex-row items-center"}>
            <input
              type="checkbox"
              id="isAdmin"
              checked={isAdminFilter}
              onChange={(e) => setIsAdminFilter(e.target.checked)}
              className="w-4 h-4 cursor-pointer accent-red-600 border-gray-600 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
            />
            <label
              htmlFor="isAdmin"
              className="ml-2 text-lg text-white font-medium cursor-pointer"
            >
              {t("user:admin")}
            </label>
          </div>
        </div>
        <div>
          <select
            className={
              "px-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent w-18 cursor-pointer"
            }
            onChange={(e) => setPageSize(Number(e.target.value))}
          >
            <option value={10}>10</option>
            <option value={25}>25</option>
            <option value={50}>50</option>
          </select>
        </div>
      </div>
      {users.length > 0 ? (
        <div className="relative overflow-visible bg-gray-800 rounded-2xl border border-gray-700 text-white ">
          <table className="w-full text-sm text-left rtl:text-right text-body">
            <thead className="{bg-gray-800/40}">
              <tr className={"px-2 bg-gray-700"}>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tl-xl"
                  }
                >
                  {t("name")}
                </th>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("contact")}
                </th>
                {/*<th scope="col" className={"py-3 font-medium"}>{t("phone")}</th>*/}
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("joined")}
                </th>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("membership")}
                </th>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("active")}
                </th>
                <th scope="col" className={"py-3 rounded-tr-xl"}></th>
              </tr>
            </thead>
            <tbody className={"divide-y divide-gray-700"}>
              {users.map((user, index) => {
                const isLast = index === users.length - 1;
                return (
                  <tr key={user.id} className={"hover:bg-gray-700"}>
                    <td
                      className={`max-w-[200px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap flex flex-row items-center align-middle h-max`}
                    >
                      <div
                        className={
                          "w-10 h-10 min-w-10 max-w-10 min-h-10 max-h-10 size-10 rounded-full bg-linear-to-br from-red-500 to-red-700 flex items-center justify-center text-white font-bold text-sm"
                        }
                      >
                        {user.name
                          .trim()
                          .split(/\s+/)
                          .map((w) => w[0].toUpperCase())
                          .join("")}
                      </div>
                      <span className={"ml-2"}>{user.name}</span>
                    </td>
                    <td className="max-w-[200px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">
                      <div className={"flex flex-row items-center gap-1"}>
                        <Mail size={16} />
                        <span>{user.email}</span>
                      </div>

                      <div
                        className={
                          "flex flex-row items-center gap-1 text-gray-500"
                        }
                      >
                        <Phone size={16} />
                        <span>{user.phoneNumber}</span>
                      </div>
                    </td>
                    <td className="max-w-[120px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">
                      {new Date(user.createdAt).toLocaleString("en-GB", {
                        year: "numeric",
                        month: "long",
                        day: "numeric",
                      })}
                    </td>
                    <td
                      className={`max-w-[120px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap ${
                        user.expiresAt && new Date(user.expiresAt) < new Date()
                          ? "text-red-600"
                          : ""
                      }`}
                    >
                      {user.expiresAt
                        ? new Date(user.expiresAt).toLocaleString("en-GB", {
                            year: "numeric",
                            month: "long",
                            day: "numeric",
                            hour: "2-digit",
                            minute: "2-digit",
                            second: "2-digit",
                          })
                        : t("not_set")}
                    </td>

                    <td
                      className={`px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap flex flex-row items-center-safe align-middle h-max`}
                    >
                      {user.isActive ? (
                        <Check size={27} color={"#48ff00"} />
                      ) : (
                        <X size={27} color={"#ff0000"} />
                      )}
                    </td>

                    <td
                      className={`w-12 px-3 py-2 ${isLast ? "rounded-br-xl" : ""}`}
                    >
                      <div className="py-2">
                        <ActionMenu
                          dropdown={true}
                          actions={[
                            {
                              label: t("edit"),
                              icon: <SquarePen color="#d0d0d0" />,
                              color: "#d0d0d0",
                              onClick: () => handleEdit(user),
                            },
                            ...(!user.isAdmin
                              ? [
                                  {
                                    label: t("renew"),
                                    color: "#00a63e",
                                    icon: <TicketCheck color="#00a63e" />,
                                    requiresConfirmation: true,
                                    onClick: () => handleRenewMembership(user),
                                  },
                                ]
                              : []),
                            ...(!user.isAdmin
                              ? [
                                  {
                                    label: t("transactions"),
                                    color: "#d0d0d0",
                                    icon: <Wallet color="#d0d0d0" />,
                                    onClick: () =>
                                      navigate(
                                        `/admin/transactions/user/${user.id}`,
                                      ),
                                  },
                                ]
                              : []),
                            ...(!user.isAdmin && !user.isActive
                              ? [
                                  {
                                    label: t("activate_user"),
                                    color: "#00a63e",
                                    icon: <Check color="#00a63e" />,
                                    requiresConfirmation: true,
                                    onClick: () => handleActivateUser(user),
                                  },
                                ]
                              : []),
                            {
                              separator: true,
                            },
                            ...(!user.isAdmin && user.isActive
                              ? [
                                  {
                                    label: t("deactivate_user"),
                                    color: "#ff0000",
                                    icon: <X color="#ff0000" />,
                                    requiresConfirmation: true,
                                    onClick: () => handleDeactivateUser(user),
                                  },
                                ]
                              : []),
                            {
                              separator: true,
                            },
                            {
                              label: t("delete"),
                              color: "#ff0000",
                              icon: <Trash2 color="#ff0000" />,
                              requiresConfirmation: true,
                              onClick: () => handleDelete(user),
                            },
                          ]}
                        />
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="flex flex-col justify-center items-center h-full text-3xl text-white">
          <UserX size={50} />
          <span>{t("no_users_found")}</span>
          <button
            onClick={handleOpenCreateModal}
            className={
              "mt-6 flex flex-row items-center justify-evenly px-3 py-1.5 rounded-lg bg-[#e30613] hover:bg-[#c20510] text-white shadow-lg hover:shadow-xl transition-all duration-300 cursor-pointer"
            }
          >
            <UserPlus size={18} className={"me-2"} />
            <span>{t("user:create_new_user")}</span>
          </button>
        </div>
      )}
      {totalPages > 1 && (
        <div className="mt-6 flex justify-center">
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={setCurrentPage}
          />
        </div>
      )}

      {/* Edit User Modal */}
      {isEditModalOpen && (
        <div className="fixed inset-0 bg-slate-700/65 flex items-center justify-center z-50">
          <div className="bg-gray-800 border border-gray-700 rounded-xl shadow-lg w-full max-w-md p-6 text-white">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-semibold">{t("user:edit_user")}</h2>
              <button
                onClick={handleCloseEditModal}
                className="text-gray-500 hover:text-gray-700 cursor-pointer"
              >
                <X size={24} />
              </button>
            </div>

            <div>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium mb-1">
                    {t("name")}
                  </label>
                  <input
                    type="text"
                    value={editForm.name}
                    onChange={(e) =>
                      setEditForm({ ...editForm, name: e.target.value })
                    }
                    className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                    required
                  />
                  {editErrors.Name && (
                    <p className="text-red-500 text-sm mt-1">
                      {editErrors.Name}
                    </p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium mb-1">
                    {t("email")}
                  </label>
                  <input
                    type="email"
                    value={editForm.email}
                    onChange={(e) =>
                      setEditForm({ ...editForm, email: e.target.value })
                    }
                    className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                    required
                  />
                  {editErrors.Email && (
                    <p className="text-red-500 text-sm mt-1">
                      {editErrors.Email}
                    </p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium mb-1">
                    {t("phone_number")}
                  </label>
                  <input
                    type="text"
                    value={editForm.phoneNumber}
                    onChange={(e) =>
                      setEditForm({ ...editForm, phoneNumber: e.target.value })
                    }
                    className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                  />
                  {editErrors.PhoneNumber && (
                    <p className="text-red-500 text-sm mt-1">
                      {editErrors.PhoneNumber}
                    </p>
                  )}
                </div>
              </div>

              <div className="flex justify-between gap-3 mt-6">
                <button
                  type="button"
                  onClick={handleCloseEditModal}
                  className="px-4 py-2 border border-gray-700 rounded-lg bg-gray-700 hover:bg-gray-600 cursor-pointer"
                >
                  {t("cancel")}
                </button>
                <button
                  type="button"
                  onClick={handleUpdateUser}
                  className="px-4 py-2 border border-gray-700 rounded-lg bg-green-500 text-white hover:bg-green-400 cursor-pointer"
                >
                  {t("save")}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Create User Modal */}
      {isCreateModalOpen && (
        <div className="fixed inset-0 bg-slate-700/65 flex items-center justify-center z-50">
          <div className="bg-gray-800 border border-gray-700 rounded-xl shadow-lg w-full max-w-md p-6 text-white">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-semibold">
                {t("user:create_new_user")}
              </h2>
              <button
                onClick={handleCloseCreateModal}
                className="text-gray-500 hover:text-gray-700 cursor-pointer"
              >
                <X size={24} />
              </button>
            </div>

            <div>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium mb-1">
                    {t("name")}
                  </label>
                  <input
                    type="text"
                    value={createForm.name}
                    onChange={(e) =>
                      setCreateForm({ ...createForm, name: e.target.value })
                    }
                    className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                    required
                  />
                  {createErrors.Name?.map((msg) => (
                    <p className="text-red-500 text-sm mt-1">
                      {t(`validation:${msg}`)}
                    </p>
                  ))}
                </div>

                <div>
                  <label className="block text-sm font-medium mb-1">
                    {t("email")}
                  </label>
                  <input
                    type="email"
                    value={createForm.email}
                    onChange={(e) =>
                      setCreateForm({ ...createForm, email: e.target.value })
                    }
                    className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                    required
                  />
                  {createErrors.Email?.map((msg) => (
                    <p className="text-red-500 text-sm mt-1">
                      {t(`validation:${msg}`)}
                    </p>
                  ))}
                </div>

                <div>
                  <label className="block text-sm font-medium mb-1">
                    {t("phone_number")}
                  </label>
                  <input
                    type="text"
                    value={createForm.phoneNumber}
                    onChange={(e) =>
                      setCreateForm({
                        ...createForm,
                        phoneNumber: e.target.value,
                      })
                    }
                    className="w-full pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                  />
                  {createErrors.PhoneNumber?.map((msg) => (
                    <p className="text-red-500 text-sm mt-1">
                      {t(`validation:${msg}`)}
                    </p>
                  ))}
                </div>

                <div className="flex items-center">
                  <input
                    type="checkbox"
                    id="activateMembership"
                    checked={createForm.activateMembership}
                    onChange={(e) =>
                      setCreateForm({
                        ...createForm,
                        activateMembership: e.target.checked,
                      })
                    }
                    className="w-4 h-4 cursor-pointer accent-red-600"
                  />
                  <label
                    htmlFor="activateMembership"
                    className="ml-2 text-sm font-medium cursor-pointer"
                  >
                    {t("user:activate_membership")}
                  </label>
                </div>

                <div className="flex items-center">
                  <input
                    type="checkbox"
                    id="isAdmin"
                    checked={createForm.isAdmin}
                    onChange={(e) =>
                      setCreateForm({
                        ...createForm,
                        isAdmin: e.target.checked,
                      })
                    }
                    className="w-4 h-4 cursor-pointer accent-red-600"
                  />
                  <label
                    htmlFor="isAdmin"
                    className="ml-2 text-sm font-medium cursor-pointer"
                  >
                    {t("user:admin_user")}
                  </label>
                </div>
              </div>

              <div className="flex justify-between gap-3 mt-6">
                <button
                  type="button"
                  onClick={handleCloseCreateModal}
                  className="px-4 py-2 border border-gray-700 rounded-lg bg-gray-700 hover:bg-gray-600 cursor-pointer"
                >
                  {t("cancel")}
                </button>
                <button
                  type="button"
                  onClick={handleCreateUser}
                  className="px-4 py-2 border border-gray-700 rounded-lg bg-green-500 text-white hover:bg-green-400 cursor-pointer"
                >
                  {t("create")}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
