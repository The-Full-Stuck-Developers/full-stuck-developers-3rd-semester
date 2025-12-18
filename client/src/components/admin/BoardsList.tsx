import { useEffect, useState } from "react";
import Pagination from "../Pagination";
import { Wallet, Mail } from "lucide-react";
import toast, { Toaster } from "react-hot-toast";
import { useTranslation } from "react-i18next";
import { ActionMenu } from "@components/ActionMenu.tsx";
import { useNavigate } from "react-router-dom";
import getBoardsClient from "@core/clients/boardsClient.ts"; // You'll need this

export default function BoardsList() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [boards, setBoards] = useState<AdminBoardDto[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [userSearchFilter, setUserSearchFilter] = useState("");
    const [pageSize, setPageSize] = useState(10);

    const buildFilterString = () => {
        const filters: string[] = [];

        if (userSearchFilter) {
            filters.push(
                `UserName@=*${userSearchFilter}|UserEmail@=*${userSearchFilter}`
            );
        }

        return filters.length > 0 ? filters.join(",") : null;
    };

    const fetchBoards = (page: number) => {
        const client = getBoardsClient();
        const filterString = buildFilterString();

        client
            .getAllBoards(filterString, page, pageSize)
            .then((res) => {
                setBoards(res.items);
                setTotalPages(Math.ceil(res.total / pageSize));
            })
            .catch((err) => {
                console.error(err);
                toast.error("Failed to load boards");
            });
    };

    useEffect(() => {
        const timer = setTimeout(() => {
            setCurrentPage(1);
            fetchBoards(1);
        }, 500);

        return () => clearTimeout(timer);
    }, [userSearchFilter, pageSize]);

    useEffect(() => {
        fetchBoards(currentPage);
    }, [currentPage]);

    const formatNumbers = (numbers: number[]) => {
        return numbers.map((n) => n.toString().padStart(2, "0"));
    };

    return (
        <div>
            <Toaster position="top-center" />
            <div className={"flex flex-row items-center justify-between w-full pb-8"}>
                <p className={"text-white text-3xl mb-2 mx-0 p-0"}>
                    {t("boards")} {/* or "Player Boards" */}
                </p>
            </div>

            {/* Filters */}
            <div className="flex flex-row justify-between bg-gray-800 rounded-2xl p-6 border border-gray-700 mb-6">
                <div className="flex flex-row gap-4">
                    <div>
                        <input
                            type="text"
                            placeholder={t("placeholders:search_user")}
                            value={userSearchFilter}
                            onChange={(e) => setUserSearchFilter(e.target.value)}
                            className="w-69 pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
                        />
                    </div>
                </div>
                <div>
                    <select
                        className="px-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent w-18 cursor-pointer"
                        onChange={(e) => setPageSize(Number(e.target.value))}
                        value={pageSize}
                    >
                        <option value={10}>10</option>
                        <option value={25}>25</option>
                        <option value={50}>50</option>
                    </select>
                </div>
            </div>

            {/* Table */}
            {boards.length > 0 ? (
                <div className="relative overflow-visible bg-gray-800 rounded-2xl border border-gray-700 text-white">
                    <table className="w-full text-sm text-left rtl:text-right text-body">
                        <thead>
                        <tr className="px-2 bg-gray-700">
                            <th className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tl-xl">
                                {t("user")}
                            </th>
                            <th className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                {t("numbers")}
                            </th>
                            <th className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                {t("count")}
                            </th>
                            <th className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                {t("total_price")}
                            </th>
                            <th className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                                {t("date")}
                            </th>
                            <th className="px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tr-xl"></th>
                        </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-700">
                        {boards.map((board, index) => {
                            const isLast = index === boards.length - 1;
                            const initials =
                                board.userName
                                    ?.trim()
                                    .split(/\s+/)
                                    .map((w) => w[0].toUpperCase())
                                    .join("") || "U";

                            return (
                                <tr key={board.id}>
                                    <td className={`px-3 py-3 ${isLast ? "rounded-bl-xl" : ""}`}>
                                        <div className="flex items-center gap-2">
                                            <div className="w-8 h-8 min-w-8 min-h-8 rounded-full bg-gradient-to-br from-red-500 to-red-700 flex items-center justify-center text-white font-bold text-xs">
                                                {initials}
                                            </div>
                                            <div className="flex flex-col">
                          <span className="text-sm font-medium">
                            {board.userName || "N/A"}
                          </span>
                                                <span className="text-xs text-gray-400 flex items-center gap-1">
                            <Mail size={12} />
                                                    {board.userEmail || "N/A"}
                          </span>
                                            </div>
                                        </div>
                                    </td>
                                    <td className="px-3 py-3">
                                        <div className="flex flex-wrap gap-2">
                                            {formatNumbers(board.numbers).map((num) => (
                                                <div
                                                    key={num}
                                                    className="w-10 h-10 bg-red-600 text-white rounded-lg flex items-center justify-center font-bold text-sm shadow"
                                                >
                                                    {num}
                                                </div>
                                            ))}
                                        </div>
                                    </td>
                                    <td className="px-3 py-3 text-center font-semibold">
                                        {board.numbersCount}
                                    </td>
                                    <td className="px-3 py-3">
                                        <div className="flex items-center gap-1 font-semibold text-green-400">
                                            <span>{board.price.toFixed(2)}</span>
                                            <span className="text-xs">kr.</span>
                                        </div>
                                    </td>
                                    <td className={`px-3 py-3 ${isLast ? "rounded-br-xl" : ""}`}>
                                        {new Date(board.createdAt).toLocaleString("en-GB", {
                                            year: "numeric",
                                            month: "short",
                                            day: "numeric",
                                            hour: "2-digit",
                                            minute: "2-digit",
                                        })}
                                    </td>
                                    <td>
                                        <ActionMenu
                                            dropdown={true}
                                            actions={[
                                                {
                                                    label: t("user") + " " + t("transactions"),
                                                    color: "#d0d0d0",
                                                    icon: <Wallet color="#d0d0d0" />,
                                                    onClick: () =>
                                                        navigate(`/admin/transactions/user/${board.userId}`),
                                                },
                                            ]}
                                        />
                                    </td>
                                </tr>
                            );
                        })}
                        </tbody>
                    </table>
                </div>
            ) : (
                <div className="flex flex-col justify-center items-center h-full text-3xl text-white py-16">
                    <Wallet size={50} className="mb-4" />
                    <span>{t("no_boards_found") || "No boards found"}</span>
                </div>
            )}

            {/* Pagination */}
            {totalPages > 1 && (
                <div className="mt-6 flex justify-center">
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