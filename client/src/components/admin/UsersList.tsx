import {useEffect, useState} from "react";
import {type CreateUserDto, type UpdateUserDto, type UserDto, UsersClient} from "@core/generated-client.ts";
import {baseUrl} from "@core/baseUrl.ts";
import Pagination from "../Pagination";
import {Check, SquarePen, Trash2, UserPlus, X} from "lucide-react";
import {ActionMenu} from "@components/ActionMenu.tsx";
import toast, {Toaster} from "react-hot-toast";
import {useTranslation} from "react-i18next";

export default function UsersList() {
    const {t} = useTranslation();
    const [users, setUsers] = useState<UserDto[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    // Filter states
    const [emailFilter, setEmailFilter] = useState("");
    const [nameFilter, setNameFilter] = useState("");
    const [phoneFilter, setPhoneFilter] = useState("");
    const [pageSize, setPageSize] = useState(10);

    // Edit modal states
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);
    const [editForm, setEditForm] = useState({
        name: "",
        email: "",
        phoneNumber: ""
    });

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
    const [createErrors, setCreateErrors] = useState<Record<string, string[]>>({});


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

        return filters.length > 0 ? filters.join(",") : null;
    };

    const getClient = () => {
        return new UsersClient(baseUrl, {
            fetch: async (url, init) => {
                init = init ?? {};
                init.headers = {
                    ...(init.headers ?? {}),
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                };
                return fetch(url, init);
            },
        });
    };

    const fetchUsers = (page: number) => {
        const client = getClient();
        const filterString = buildFilterString();

        client.getAllUsers(filterString, null, page, pageSize)
            .then(res => {
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
    }, [emailFilter, nameFilter, phoneFilter, pageSize]);

    useEffect(() => {
        fetchUsers(currentPage);
    }, [currentPage]);

    const handleDelete = (user: UserDto) => {
        const client = getClient();

        client.deleteUser(user.id!)
            .then(() => fetchUsers(currentPage))
            .catch(console.error);
    };

    const handleEdit = (user: UserDto) => {
        setSelectedUser(user);
        setEditForm({
            name: user.name || "",
            email: user.email || "",
            phoneNumber: user.phoneNumber || ""
        });
        setIsEditModalOpen(true);
    };

    const handleCloseEditModal = () => {
        setIsEditModalOpen(false);
        setSelectedUser(null);
        setEditForm({
            name: "",
            email: "",
            phoneNumber: ""
        });
    };

    const handleUpdateUser = async (e: React.MouseEvent) => {
        e.preventDefault();

        if (!selectedUser?.id) return;

        const client = getClient();
        const updateDto: UpdateUserDto = {
            name: editForm.name,
            email: editForm.email,
            phoneNumber: editForm.phoneNumber
        };

        try {
            await client.updateUser(selectedUser.id, updateDto);
            fetchUsers(currentPage);
            handleCloseEditModal();
            toast.success("User updated successfully.");
        } catch (error) {
            console.error("Error updating user:", error);
            toast.error("Error updating user, please try again.",);
        }
    };

    const handleOpenCreateModal = () => {
        setIsCreateModalOpen(true);
    };

    const handleCloseCreateModal = () => {
        console.log('asdasd');
        setIsCreateModalOpen(false);
        setCreateForm({
            name: "",
            email: "",
            phoneNumber: "",
            password: "",
            isAdmin: false,
            activateMembership: false,
        });
    };

    const handleCreateUser = async (e: React.MouseEvent) => {
        e.preventDefault();

        const client = getClient();
        const createDto: CreateUserDto = {
            name: createForm.name,
            email: createForm.email,
            phoneNumber: createForm.phoneNumber,
            isAdmin: createForm.isAdmin,
            activateMembership: createForm.activateMembership || false
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

                Object.keys(validation).forEach(key => {
                    formatted[key] = validation[key];
                });

                setCreateErrors(formatted);
            } else {
                toast.error("Something went wrong, please try again later.");
            }
        }
    };

    const handleRenewMembership = (user: UserDto) => {
        const client = getClient();

        client.renewMembership(user.id!)
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
                    toast.error(error.message || 'An error occurred');
                    return;
                }
            });
    };

    useEffect(() => {
        const handleEscape = (e: KeyboardEvent) => {
            if (e.key === 'Escape' && (isEditModalOpen || isCreateModalOpen)) {
                handleCloseCreateModal();
                handleCloseEditModal();
            }
        };

        document.addEventListener('keydown', handleEscape);
        return () => document.removeEventListener('keydown', handleEscape);
    }, [isEditModalOpen, isCreateModalOpen]);

    return (
        <div>
            <Toaster position="top-center"/>
            <div className={"flex flex-row items-center justify-between w-full pb-8"}>
                <p className={"text-3xl mb-2 mx-0 p-0"}>{t("users")}</p>
                <button
                    onClick={handleOpenCreateModal}
                    className={"px-3 py-1.5 bg-slate-300 rounded-md hover:bg-slate-400 flex flex-row items-center outline cursor-pointer"}>
                    <UserPlus size={18} className={"me-2"}/>
                    <span>
                        {t("user:create_new_user")}
                    </span>
                </button>
            </div>
            <div className={"flex flex-row justify-between"}>
                <div className={"flex flex-row gap-4 pb-3"}>
                    <div className={""}>
                        <input
                            type="email"
                            placeholder={t("placeholders:enter_email")}
                            value={emailFilter}
                            required={true}
                            onChange={(e) => setEmailFilter(e.target.value)}
                            className="input input-neutral border rounded-xl px-4 py-1.5 text-lg w-full focus:outline-verde"
                        />
                    </div>
                    <div>
                        <input
                            type="text"
                            placeholder={t("placeholders:enter_name")}
                            value={nameFilter}
                            required={true}
                            onChange={(e) => setNameFilter(e.target.value)}
                            className="input input-neutral border rounded-xl px-4 py-1.5 text-lg w-full focus:outline-verde"
                        />
                    </div>
                    <div>
                        <input
                            type="text"
                            placeholder={t("placeholders:enter_phone")}
                            value={phoneFilter}
                            required={true}
                            onChange={(e) => setPhoneFilter(e.target.value)}
                            className="input input-neutral border rounded-xl px-4 py-1.5 text-lg w-full focus:outline-verde"
                        />
                    </div>
                </div>
                <div>
                    <select
                        className={"select z-20 select-neutral border rounded-xl px-4 py-1.5 text-lg w-20 focus:outline-verde"}
                        onChange={(e) => setPageSize(Number(e.target.value))}>
                        <option value={10}>10</option>
                        <option value={25}>25</option>
                        <option value={50}>50</option>
                    </select>
                </div>
            </div>
            <div
                className="relative overflow-visible bg-neutral-primary-soft shadow-xs rounded-base border border-default w-full rounded-xl">
                <table className="w-full text-sm text-left rtl:text-right text-body">
                    <thead className="{text-sm px-2 text-body border-b rounded-base border-default rounded-t-xl}">
                    <tr className={"bg-slate-300 px-2"}>
                        <th scope="col" className={"px-3 py-3 font-medium rounded-tl-xl"}>{t("name")}</th>
                        <th scope="col" className={"py-3 font-medium"}>{t("email")}</th>
                        <th scope="col" className={"py-3 font-medium"}>{t("phone")}</th>
                        <th scope="col" className={"py-3 font-medium"}>{t("joined")}</th>
                        <th scope="col" className={"py-3 font-medium"}>{t("membership")}</th>
                        <th scope="col" className={"py-3  rounded-tr-xl"}></th>
                    </tr>
                    </thead>
                    <tbody>
                    {users.map((user, index) => {
                        const isLast = index === users.length - 1;
                        return (
                            <tr key={user.id}
                                className={`border-b border-default even:bg-slate-200 ${isLast ? "last:border-0" : ""}`}>
                                <td className={`max-w-[120px] px-3 py-2 text-ellipsis ${isLast ? "rounded-bl-xl" : ""}`}>{user.name}</td>
                                <td className="max-w-[200px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">{user.email}</td>
                                <td className="max-w-[120px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">{user.phoneNumber}</td>
                                <td className="max-w-[120px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">{new Date(user.createdAt).toLocaleString("en-GB", {
                                    year: "numeric",
                                    month: "long",
                                    day: "numeric",
                                    hour: "2-digit",
                                    minute: "2-digit",
                                    second: "2-digit"
                                })}</td>
                                <td
                                    className={`max-w-[120px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap ${
                                        user.expiresAt && new Date(user.expiresAt) < new Date() ? "text-red-600" : ""
                                    }`}
                                >
                                    {user.expiresAt
                                        ? new Date(user.expiresAt).toLocaleString("en-GB", {
                                            year: "numeric",
                                            month: "long",
                                            day: "numeric",
                                            hour: "2-digit",
                                            minute: "2-digit",
                                            second: "2-digit"
                                        })
                                        : "Not Set"}
                                </td>

                                <td className={`w-12 px-3 py-2 ${isLast ? "rounded-br-xl" : ""}`}>
                                    <div className="py-2">
                                        <ActionMenu
                                            actions={[
                                                {
                                                    label: t("edit"),
                                                    icon: <SquarePen/>,
                                                    onClick: () => handleEdit(user)
                                                },
                                                {
                                                    separator: true
                                                },
                                                {
                                                    label: t("renew"),
                                                    color: "#00a63e",
                                                    icon: <Check color="#00a63e"/>,
                                                    requiresConfirmation: true,
                                                    onClick: () => handleRenewMembership(user)
                                                },
                                                {
                                                    separator: true
                                                },
                                                {
                                                    label: t("delete"),
                                                    color: "#ff0000",
                                                    icon: <Trash2 color="#ff0000"/>,
                                                    requiresConfirmation: true,
                                                    onClick: () => handleDelete(user)
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
            {totalPages > 1 && (
                <div className="mt-4 flex justify-center">
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
                    <div className="bg-white rounded-xl shadow-lg w-full max-w-md p-6">
                        <div className="flex justify-between items-center mb-4">
                            <h2 className="text-2xl font-semibold">{t("user:edit_user")}</h2>
                            <button
                                onClick={handleCloseEditModal}
                                className="text-gray-500 hover:text-gray-700 cursor-pointer"
                            >
                                <X size={24}/>
                            </button>
                        </div>

                        <div>
                            <div className="space-y-4">
                                <div>
                                    <label className="block text-sm font-medium mb-1">{t("name")}</label>
                                    <input
                                        type="text"
                                        value={createForm.name}
                                        onChange={(e) => setCreateForm({...createForm, name: e.target.value})}
                                        className="w-full border rounded-lg px-3 py-2 focus:outline-verde"
                                        required
                                    />
                                    {createErrors.Name &&
                                        <p className="text-red-500 text-sm mt-1">{createErrors.Name}</p>}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium mb-1">{t("email")}</label>
                                    <input
                                        type="email"
                                        value={createForm.email}
                                        onChange={(e) => setCreateForm({...createForm, email: e.target.value})}
                                        className="w-full border rounded-lg px-3 py-2 focus:outline-verde"
                                        required
                                    />
                                    {createErrors.Email &&
                                        <p className="text-red-500 text-sm mt-1">{createErrors.Email}</p>}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium mb-1">{t("phone_number")}</label>
                                    <input
                                        type="text"
                                        value={createForm.phoneNumber}
                                        onChange={(e) => setCreateForm({...createForm, phoneNumber: e.target.value})}
                                        className="w-full border rounded-lg px-3 py-2 focus:outline-verde"
                                    />
                                    {createErrors.PhoneNumber &&
                                        <p className="text-red-500 text-sm mt-1">{createErrors.PhoneNumber}</p>}
                                </div>
                            </div>

                            <div className="flex justify-between gap-3 mt-6">
                                <button
                                    type="button"
                                    onClick={handleCloseEditModal}
                                    className="px-4 py-2 border rounded-lg hover:bg-gray-100 cursor-pointer"
                                >
                                    {t("cancel")}
                                </button>
                                <button
                                    type="button"
                                    onClick={handleUpdateUser}
                                    className="px-4 py-2 border rounded-lg bg-green-500 text-white hover:bg-green-400 cursor-pointer"
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
                    <div className="bg-white rounded-xl shadow-lg w-full max-w-md p-6">
                        <div className="flex justify-between items-center mb-4">
                            <h2 className="text-2xl font-semibold">{t("user:create_new_user")}</h2>
                            <button
                                onClick={handleCloseCreateModal}
                                className="text-gray-500 hover:text-gray-700 cursor-pointer"
                            >
                                <X size={24}/>
                            </button>
                        </div>

                        <div>
                            <div className="space-y-4">
                                <div>
                                    <label className="block text-sm font-medium mb-1">{t("name")}</label>
                                    <input
                                        type="text"
                                        value={createForm.name}
                                        onChange={(e) => setCreateForm({...createForm, name: e.target.value})}
                                        className="w-full border rounded-lg px-3 py-2 focus:outline-verde"
                                        required
                                    />
                                    {createErrors.Name?.map((msg) => (
                                        <p className="text-red-500 text-sm mt-1">{msg}</p>
                                    ))}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium mb-1">{t("email")}</label>
                                    <input
                                        type="email"
                                        value={createForm.email}
                                        onChange={(e) => setCreateForm({...createForm, email: e.target.value})}
                                        className="w-full border rounded-lg px-3 py-2 focus:outline-verde"
                                        required
                                    />
                                    {createErrors.Email?.map((msg) => (
                                        <p className="text-red-500 text-sm mt-1">{msg}</p>
                                    ))}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium mb-1">{t("phone_number")}</label>
                                    <input
                                        type="text"
                                        value={createForm.phoneNumber}
                                        onChange={(e) => setCreateForm({...createForm, phoneNumber: e.target.value})}
                                        className="w-full border rounded-lg px-3 py-2 focus:outline-verde"
                                    />
                                    {createErrors.PhoneNumber?.map((msg) => (
                                        <p className="text-red-500 text-sm mt-1">{msg}</p>
                                    ))}
                                </div>


                                <div className="flex items-center">
                                    <input
                                        type="checkbox"
                                        id="activateMembership"
                                        checked={createForm.activateMembership}
                                        onChange={(e) => setCreateForm({
                                            ...createForm,
                                            activateMembership: e.target.checked
                                        })}
                                        className="w-4 h-4 cursor-pointer"
                                    />
                                    <label htmlFor="activateMembership"
                                           className="ml-2 text-sm font-medium cursor-pointer">
                                        {t("user:activate_membership")}
                                    </label>
                                </div>

                                <div className="flex items-center">
                                    <input
                                        type="checkbox"
                                        id="isAdmin"
                                        checked={createForm.isAdmin}
                                        onChange={(e) => setCreateForm({...createForm, isAdmin: e.target.checked})}
                                        className="w-4 h-4 cursor-pointer"
                                    />
                                    <label htmlFor="isAdmin" className="ml-2 text-sm font-medium cursor-pointer">
                                        {t("user:admin_user")}
                                    </label>
                                </div>
                            </div>

                            <div className="flex justify-between gap-3 mt-6">
                                <button
                                    type="button"
                                    onClick={handleCloseCreateModal}
                                    className="px-4 py-2 border rounded-lg hover:bg-gray-100 cursor-pointer"
                                >
                                    {t("cancel")}
                                </button>
                                <button
                                    type="button"
                                    onClick={handleCreateUser}
                                    className="px-4 py-2 border rounded-lg bg-green-500 text-white hover:bg-green-400 cursor-pointer"
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