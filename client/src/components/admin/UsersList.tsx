import { useEffect, useState } from "react";
import { UsersClient, type UserDto } from "../../core/generated-client.ts";
import { baseUrl } from "@core/baseUrl.ts";
import Pagination from "../Pagination";

export default function UsersList() {
    const [users, setUsers] = useState<UserDto[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    const pageSize = 10;

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

        client.getAllUsers(null, null, page, pageSize)
            .then(res => {
                setUsers(res.items);
                setTotalPages(Math.ceil(res.total / pageSize));
            })
            .catch(console.error);
    };

    useEffect(() => {
        fetchUsers(currentPage);
    }, [currentPage]);

    return (
        <div className="p-4">
            <table className="table w-full table-zebra">
                <thead>
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Email</th>
                </tr>
                </thead>
                <tbody>
                {users.map(user => (
                    <tr key={user.id}>
                        <td>{user.id}</td>
                        <td>{user.name}</td>
                        <td>{user.email}</td>
                    </tr>
                ))}
                </tbody>
            </table>

            <div className="mt-4 flex justify-center">
                <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    onPageChange={setCurrentPage}
                />
            </div>
        </div>
    );
}
