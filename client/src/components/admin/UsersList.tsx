import {useEffect, useState} from "react";
import {type UserDto, UsersClient} from "@core/generated-client.ts";
import {baseUrl} from "@core/baseUrl.ts";
import Pagination from "../Pagination";
import {SquarePen, Trash2} from "lucide-react";
import {ActionMenu} from "@components/ActionMenu.tsx";

export default function UsersList() {
    const [users, setUsers] = useState<UserDto[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    // Filter states
    const [emailFilter, setEmailFilter] = useState("");
    const [nameFilter, setNameFilter] = useState("");
    const [phoneFilter, setPhoneFilter] = useState("");
    const [pageSize, setPageSize] = useState(10);

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

    const fetchUsers = (page: number) => {
        const client = new UsersClient(baseUrl, {
            fetch: async (url, init) => {
                init = init ?? {};
                init.headers = {
                    ...(init.headers ?? {}),
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                };
                return fetch(url, init);
            },
        });

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
        const client = new UsersClient(baseUrl, {
            fetch: async (url, init) => {
                init = init ?? {};
                init.headers = {
                    ...(init.headers ?? {}),
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                };
                return fetch(url, init);
            },
        });

        client.deleteUser(user.id!)
            .then(() => fetchUsers(currentPage))
            .catch(console.error);
    };

    const handleEdit = (user: UserDto) => {
        return undefined;
    };

    return (
        <div>
            <p className={"text-3xl mb-2 mx-0 p-0"}>Users</p>
            <div className={"flex flex-row justify-between"}>
                <div className={"flex flex-row gap-4 pb-3"}>
                    <div className={""}>
                        <input
                            type="email"
                            placeholder="Enter email"
                            value={emailFilter}
                            onChange={(e) => setEmailFilter(e.target.value)}
                            className="input input-neutral border rounded-xl px-4 py-1.5 text-lg w-full focus:outline-verde"
                        />
                    </div>
                    <div>
                        <input
                            type="text"
                            placeholder="Enter name"
                            value={nameFilter}
                            onChange={(e) => setNameFilter(e.target.value)}
                            className="input input-neutral border rounded-xl px-4 py-1.5 text-lg w-full focus:outline-verde"
                        />
                    </div>
                    <div>
                        <input
                            type="text"
                            placeholder="Enter phone"
                            value={phoneFilter}
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
                        <th scope="col" className={"px-3 py-3 font-medium rounded-tl-xl"}>ID</th>
                        <th scope="col" className={"py-3 font-medium"}>Name</th>
                        <th scope="col" className={"py-3 font-medium"}>Email</th>
                        <th scope="col" className={"py-3 font-medium"}>Phone</th>
                        <th scope="col" className={"py-3  rounded-tr-xl"}></th>
                    </tr>
                    </thead>
                    <tbody>
                    {users.map((user, index) => {
                        const isLast = index === users.length - 1;
                        return (
                            <tr key={user.id}
                                className={`border-b border-default even:bg-slate-200 ${isLast ? "last:border-0" : ""}`}>
                                <td className={`max-w-[120px] px-3 py-2 text-ellipsis ${isLast ? "rounded-bl-xl" : ""}`}>{user.id}</td>
                                <td className="max-w-[90px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">{user.name}</td>
                                <td className="max-w-[200px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">{user.email}</td>
                                <td className="max-w-[120px] px-3 py-2 overflow-hidden text-ellipsis whitespace-nowrap">{user.phoneNumber}</td>
                                <td className={`w-12 px-3 py-2 ${isLast ? "rounded-br-xl" : ""}`}>
                                    <div className="py-2">
                                        <ActionMenu
                                            actions={[
                                                {label: "Edit", icon: <SquarePen/>, onClick: () => handleEdit(user)},
                                                {separator: true},
                                                {
                                                    label: "Delete",
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
        </div>
    );
}